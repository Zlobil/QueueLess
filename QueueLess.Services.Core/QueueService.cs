namespace QueueLess.Services
{
    using Microsoft.EntityFrameworkCore;
    using QueueLess.Data;
    using QueueLess.Data.Models;
    using QueueLess.Data.Models.Enums;
    using QueueLess.Services.Contracts;
    using QueueLess.ViewModels.Queue;
    using QueueLess.ViewModels.ServiceLocation;

    /// <summary>
    /// Service responsible for managing queues, including creation, editing, deletion,
    /// serving entries, tracking waiting clients, and providing public views.
    /// </summary>
    public class QueueService : IQueueService
    {
        private readonly ApplicationDbContext context;

        public QueueService(ApplicationDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Retrieves all queues owned by a specific user.
        /// </summary>
        /// <param name="ownerId">The ID of the user who owns the queues.</param>
        /// <returns>A collection of MyQueuesViewModel objects.</returns>
        public async Task<IEnumerable<MyQueuesViewModel>> GetAllQueuesAsync(string ownerId)
        {
            return await context.Queues
                .Where(q => q.OwnerId == ownerId)
                .OrderByDescending(q => q.CreatedOn)
                .Select(q => new MyQueuesViewModel
                {
                    Id = q.Id,
                    Name = q.Name,
                    Description = q.Description,
                    IsOpen = q.IsOpen,
                    CreatedOn = q.CreatedOn,
                    AverageServiceTimeMinutes = q.AverageServiceTimeMinutes
                })
                .ToListAsync();
        }

        /// <summary>
        /// Gets the view model for creating a new queue, including available service locations.
        /// </summary>
        /// <returns>A QueueCreateViewModel object with service locations.</returns>
        public async Task<QueueCreateViewModel> GetQueueCreateViewModelAsync()
        {
            return new QueueCreateViewModel
            {
                ServiceLocations = await context.ServiceLocations
                    .OrderBy(sl => sl.Name)
                    .Select(sl => new ServiceLocationSelectViewModel
                    {
                        Id = sl.Id,
                        Name = sl.Name
                    })
                    .ToListAsync()
            };
        }

        /// <summary>
        /// Checks whether a service location exists.
        /// </summary>
        /// <param name="id">Service location ID.</param>
        /// <returns>True if exists, false otherwise.</returns>
        public async Task<bool> ServiceLocationExistsAsync(int id)
            => await context.ServiceLocations.AnyAsync(sl => sl.Id == id);

        /// <summary>
        /// Creates a new queue for a given owner.
        /// </summary>
        /// <param name="model">Queue creation data.</param>
        /// <param name="ownerId">ID of the user creating the queue.</param>
        public async Task CreateQueueAsync(QueueCreateViewModel model, string ownerId)
        {
            var queue = new Queue
            {
                Name = model.Name,
                Description = model.Description,
                AverageServiceTimeMinutes = model.AverageServiceTimeMinutes,
                MaxWaitMinutes = model.MaxWaitMinutes,
                IsOpen = model.IsOpen,
                CreatedOn = DateTime.UtcNow,
                ServiceLocationId = model.ServiceLocationId,
                OwnerId = ownerId
            };
            context.Queues.Add(queue);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves detailed information about a specific queue.
        /// </summary>
        /// <param name="id">Queue ID.</param>
        /// <param name="ownerId">Owner's user ID.</param>
        /// <param name="tab">Optional active tab ("waiting" or "history").</param>
        /// <returns>A QueueDetailsViewModel or null if not found.</returns>
        public async Task<QueueDetailsViewModel?> GetQueueDetailsAsync(int id, string ownerId, string? tab)
        {
            var queue = await context.Queues
                .Include(q => q.QueueEntries)
                .FirstOrDefaultAsync(q => q.Id == id && q.OwnerId == ownerId);

            if (queue == null) return null;

            var waiting = queue.QueueEntries
                .Where(e => e.Status == QueueEntryStatus.Waiting ||
                            e.Status == QueueEntryStatus.Serving)
                .OrderBy(e => e.JoinedOn)
                .Select((e, index) => new QueueDetailsWaitingViewModel
                {
                    EntryId = e.Id,
                    Position = index + 1,
                    ClientName = e.ClientName,
                    Status = e.Status,
                    JoinedOn = e.JoinedOn
                })
                .ToList();

            var history = queue.QueueEntries
                .Where(e => e.Status == QueueEntryStatus.Served ||
                            e.Status == QueueEntryStatus.Skipped ||
                            e.Status == QueueEntryStatus.Expired)
                .OrderByDescending(e => e.JoinedOn)
                .Select(e => new QueueDetailsHistoryViewModel
                {
                    EntryId = e.Id,
                    ClientName = e.ClientName,
                    Status = e.Status,
                    JoinedOn = e.JoinedOn
                })
                .ToList();

            return new QueueDetailsViewModel
            {
                QueueId = queue.Id,
                Name = queue.Name,
                Description = queue.Description,
                IsOpen = queue.IsOpen,
                AverageServiceTimeMinutes = queue.AverageServiceTimeMinutes,
                CreatedOn = queue.CreatedOn,
                Entries = waiting,
                History = history,
                ActiveTab = tab ?? "waiting"
            };
        }

        /// <summary>
        /// Checks the specified queue to see if any entry is currently being served.
        /// If no entry is being served, it marks the next waiting entry as serving.
        /// This method is intended for internal use only.
        /// </summary>
        /// <param name="id">The ID of the queue to update.</param>
        private async Task ServingAsync(int id)
        {
            var queue = await context.Queues
                .Include(q => q.QueueEntries)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (queue == null) return;

            var serving = queue.QueueEntries
                .FirstOrDefault(e => e.Status == QueueEntryStatus.Serving);
            if (serving != null) return;

            var next = queue.QueueEntries
                .Where(e => e.Status == QueueEntryStatus.Waiting)
                .OrderBy(e => e.JoinedOn)
                .FirstOrDefault();
            if (next != null)
            {
                next.Status = QueueEntryStatus.Serving;
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Marks a specific queue entry as served.
        /// </summary>
        /// <param name="entryId">Queue entry ID.</param>
        public async Task ServeEntryAsync(int entryId)
        {
            var entry = await context.QueueEntries.FirstOrDefaultAsync(e => e.Id == entryId);
            if (entry == null) return;

            entry.Status = QueueEntryStatus.Served;
            await context.SaveChangesAsync();

            await ServingAsync(entry.QueueId);
        }

        /// <summary>
        /// Marks the next waiting entry in the queue as served.
        /// </summary>
        /// <param name="queueId">Queue ID.</param>
        public async Task ServeNextAsync(int queueId)
        {
            var queue = await context.Queues
                .Include(q => q.QueueEntries)
                .FirstOrDefaultAsync(q => q.Id == queueId);
            if (queue == null) return;

            var current = queue.QueueEntries
                .FirstOrDefault(e => e.Status == QueueEntryStatus.Serving);
            if (current != null)
                current.Status = QueueEntryStatus.Served;

            var next = queue.QueueEntries
                .Where(e => e.Status == QueueEntryStatus.Waiting)
                .OrderBy(e => e.JoinedOn)
                .FirstOrDefault();
            if (next != null)
                next.Status = QueueEntryStatus.Serving;

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Skips a specific queue entry.
        /// </summary>
        /// <param name="entryId">Queue entry ID.</param>
        public async Task SkipEntryAsync(int entryId)
        {
            var entry = await context.QueueEntries.FirstOrDefaultAsync(e => e.Id == entryId);
            if (entry == null) return;

            entry.Status = QueueEntryStatus.Skipped;
            await context.SaveChangesAsync();

            await ServingAsync(entry.QueueId);
        }

        /// <summary>
        /// Cleans up historical entries of a queue based on specified criteria.
        /// </summary>
        /// <param name="model">Cleanup criteria (days limit).</param>
        public async Task CleanupHistoryAsync(QueueHistoryCleanupViewModel model)
        {
            var query = context.QueueEntries
                .Where(e => e.QueueId == model.QueueId && e.Status != QueueEntryStatus.Waiting);

            if (model.Days != -1)
            {
                var cutoff = DateTime.UtcNow.AddDays(-model.Days);
                query = query.Where(e => e.JoinedOn < cutoff);
            }

            context.QueueEntries.RemoveRange(query);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a specific history entry from a queue.
        /// </summary>
        /// <param name="entryId">Queue entry ID.</param>
        /// <returns>The ID of the associated queue.</returns>
        public async Task<int> DeleteHistoryEntryAsync(int entryId)
        {
            var entry = await context.QueueEntries.FindAsync(entryId);
            if (entry == null) return 0;

            int queueId = entry.QueueId;
            context.QueueEntries.Remove(entry);
            await context.SaveChangesAsync();
            return queueId;
        }

        /// <summary>
        /// Gets a queue for editing.
        /// </summary>
        /// <param name="id">Queue ID.</param>
        /// <param name="ownerId">Owner's user ID.</param>
        /// <returns>A QueueEditViewModel or null if not found.</returns>
        public async Task<QueueEditViewModel?> GetQueueForEditAsync(int id, string ownerId)
        {
            return await context.Queues
                .Where(q => q.Id == id && q.OwnerId == ownerId)
                .Select(q => new QueueEditViewModel
                {
                    Id = q.Id,
                    Name = q.Name,
                    Description = q.Description,
                    AverageServiceTimeMinutes = q.AverageServiceTimeMinutes,
                    MaxWaitMinutes = q.MaxWaitMinutes,
                    IsOpen = q.IsOpen
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Edits an existing queue.
        /// </summary>
        /// <param name="model">Queue edit data.</param>
        /// <param name="ownerId">Owner's user ID.</param>
        public async Task EditQueueAsync(QueueEditViewModel model, string ownerId)
        {
            var queue = await context.Queues
                .FirstOrDefaultAsync(q => q.Id == model.Id && q.OwnerId == ownerId);
            if (queue == null) return;

            queue.Name = model.Name;
            queue.Description = model.Description;
            queue.AverageServiceTimeMinutes = model.AverageServiceTimeMinutes;
            queue.MaxWaitMinutes = model.MaxWaitMinutes;
            queue.IsOpen = model.IsOpen;

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets a queue for deletion confirmation.
        /// </summary>
        /// <param name="id">Queue ID.</param>
        /// <param name="ownerId">Owner's user ID.</param>
        /// <returns>A QueueDeleteViewModel or null if not found.</returns>
        public async Task<QueueDeleteViewModel?> GetQueueForDeleteAsync(int id, string ownerId)
        {
            return await context.Queues
                .Where(q => q.Id == id && q.OwnerId == ownerId)
                .Select(q => new QueueDeleteViewModel
                {
                    Id = q.Id,
                    Name = q.Name,
                    Description = q.Description
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Deletes a queue if it has no active entries.
        /// </summary>
        /// <param name="id">Queue ID.</param>
        /// <param name="ownerId">Owner's user ID.</param>
        /// <returns>True if deleted successfully, false otherwise.</returns>
        public async Task<bool> DeleteQueueAsync(int id, string ownerId)
        {
            var queue = await context.Queues
                .FirstOrDefaultAsync(q => q.Id == id && q.OwnerId == ownerId);
            if (queue == null) return false;

            bool hasEntries = await context.QueueEntries.AnyAsync(e => e.QueueId == id);
            if (hasEntries) return false;

            context.Queues.Remove(queue);
            await context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Retrieves all active queues (open for clients).
        /// </summary>
        /// <returns>Collection of QueueActiveViewModel objects.</returns>
        public async Task<IEnumerable<QueueActiveViewModel>> GetActiveQueuesAsync()
        {
            return await context.Queues
                .Where(q => q.IsOpen)
                .OrderBy(q => q.CreatedOn)
                .Select(q => new QueueActiveViewModel
                {
                    Id = q.Id,
                    Name = q.Name,
                    Description = q.Description,
                    AverageServiceTimeMinutes = q.AverageServiceTimeMinutes,
                    IsOpen = q.IsOpen
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves public view information for a queue.
        /// </summary>
        /// <param name="id">Queue ID.</param>
        /// <returns>A QueuePublicViewModel or null if not found.</returns>
        public async Task<QueuePublicViewModel?> GetPublicQueueAsync(int id)
        {
            return await context.Queues
                .Where(q => q.Id == id)
                .Select(q => new QueuePublicViewModel
                {
                    QueueId = q.Id,
                    Name = q.Name,
                    Description = q.Description,
                    IsOpen = q.IsOpen,
                    AverageServiceTimeMinutes = q.AverageServiceTimeMinutes,
                    WaitingCount = q.QueueEntries
                        .Count(e => e.Status == QueueEntryStatus.Waiting),
                    EstimatedWaitingTimeMinutes =
                        q.QueueEntries.Count(e => e.Status == QueueEntryStatus.Waiting) *
                        q.AverageServiceTimeMinutes
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves the join page view model for a queue.
        /// </summary>
        /// <param name="id">Queue ID.</param>
        /// <returns>A QueueJoinViewModel or null if not found.</returns>
        public async Task<QueueJoinViewModel?> GetQueueJoinViewModelAsync(int id)
        {
            var queue = await context.Queues.FirstOrDefaultAsync(q => q.Id == id);
            if (queue == null) return null;

            return new QueueJoinViewModel
            {
                QueueId = queue.Id,
                QueueName = queue.Name
            };
        }

        /// <summary>
        /// Adds a client to the queue.
        /// </summary>
        /// <param name="model">QueueJoinViewModel with client information.</param>
        /// <returns>The ID of the newly created queue entry.</returns>
        public async Task<int> JoinQueueAsync(QueueJoinViewModel model)
        {
            var entry = new QueueEntry
            {
                QueueId = model.QueueId,
                ClientName = model.ClientName,
                JoinedOn = DateTime.UtcNow,
                Status = QueueEntryStatus.Waiting
            };
            context.QueueEntries.Add(entry);
            await context.SaveChangesAsync();

            await ServingAsync(model.QueueId);
            return entry.Id;
        }

        /// <summary>
        /// Retrieves the waiting view model for a specific queue entry.
        /// </summary>
        /// <param name="id">Queue ID.</param>
        /// <param name="entryId">Queue entry ID.</param>
        /// <returns>A QueueWaitingViewModel or null if not found or served/skipped.</returns>
        public async Task<QueueWaitingViewModel?> GetWaitingViewModelAsync(int id, int entryId)
        {
            await ServingAsync(id);

            var queue = await context.Queues
                .Include(q => q.QueueEntries)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (queue == null) return null;

            var entry = queue.QueueEntries.FirstOrDefault(e => e.Id == entryId);
            if (entry == null) return null;

            if (entry.Status != QueueEntryStatus.Waiting &&
                entry.Status != QueueEntryStatus.Serving)
                return null;

            var ordered = queue.QueueEntries
                .Where(e => e.Status == QueueEntryStatus.Waiting ||
                            e.Status == QueueEntryStatus.Serving)
                .OrderBy(e => e.JoinedOn)
                .ToList();

            var position = ordered.FindIndex(e => e.Id == entry.Id) + 1;
            var ahead = Math.Max(position - 1, 0);
            var estimated = ahead * queue.AverageServiceTimeMinutes;

            return new QueueWaitingViewModel
            {
                QueueId = queue.Id,
                EntryId = entry.Id,
                QueueName = queue.Name,
                Position = position,
                PeopleAhead = ahead,
                EstimatedWaitMinutes = estimated
            };
        }

        /// <summary>
        /// Retrieves the current waiting status for a specific queue entry.
        /// </summary>
        /// <param name="id">Queue ID.</param>
        /// <param name="entryId">Queue entry ID.</param>
        /// <returns>An anonymous object representing the current state.</returns>
        public async Task<object> GetWaitingStatusAsync(int id, int entryId)
        {
            var queue = await context.Queues
                .Include(q => q.QueueEntries)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (queue == null)
                return new { state = "queue_removed" };

            var entry = queue.QueueEntries
                .FirstOrDefault(e => e.Id == entryId);
            if (entry == null)
                return new { state = "removed" };

            if (entry.Status == QueueEntryStatus.Serving)
                return new { state = "your_turn", position = 1, ahead = 0, estimated = 0 };

            if (entry.Status != QueueEntryStatus.Waiting)
                return new { state = entry.Status.ToString().ToLower() };

            var active = queue.QueueEntries
                .Where(e => e.Status == QueueEntryStatus.Waiting ||
                            e.Status == QueueEntryStatus.Serving)
                .OrderBy(e => e.JoinedOn)
                .ToList();

            var position = active.FindIndex(e => e.Id == entry.Id) + 1;
            var ahead = Math.Max(position - 1, 0);
            var estimated = ahead * queue.AverageServiceTimeMinutes;

            return new { state = "waiting", position, ahead, estimated };
        }

        /// <summary>
        /// Gets the queue ID for a specific entry.
        /// </summary>
        /// <param name="entryId">Queue entry ID.</param>
        /// <returns>The queue ID.</returns>
        public async Task<int> GetQueueIdByEntryAsync(int entryId)
        {
            return await context.QueueEntries
                .Where(e => e.Id == entryId)
                .Select(e => e.QueueId)
                .FirstOrDefaultAsync();
        }
    }
}
