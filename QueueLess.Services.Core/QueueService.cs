namespace QueueLess.Services
{
    using Microsoft.EntityFrameworkCore;
    using QueueLess.Data;
    using QueueLess.Data.Models;
    using QueueLess.Data.Models.Enums;
    using QueueLess.Services.Contracts;
    using QueueLess.ViewModels.Queue;
    using QueueLess.ViewModels.ServiceLocation;

    public class QueueService : IQueueService
    {
        private readonly ApplicationDbContext context;

        public QueueService(ApplicationDbContext context)
        {
            this.context = context;
        }

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

        public async Task<bool> ServiceLocationExistsAsync(int id)
            => await context.ServiceLocations.AnyAsync(sl => sl.Id == id);

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

        public async Task ServeEntryAsync(int entryId)
        {
            var entry = await context.QueueEntries.FirstOrDefaultAsync(e => e.Id == entryId);
            if (entry == null) return;

            entry.Status = QueueEntryStatus.Served;
            await context.SaveChangesAsync();

            await ServingAsync(entry.QueueId);
        }

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

        public async Task SkipEntryAsync(int entryId)
        {
            var entry = await context.QueueEntries.FirstOrDefaultAsync(e => e.Id == entryId);
            if (entry == null) return;

            entry.Status = QueueEntryStatus.Skipped;
            await context.SaveChangesAsync();

            await ServingAsync(entry.QueueId);
        }

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

        public async Task<int> DeleteHistoryEntryAsync(int entryId)
        {
            var entry = await context.QueueEntries.FindAsync(entryId);
            if (entry == null) return 0;

            int queueId = entry.QueueId;
            context.QueueEntries.Remove(entry);
            await context.SaveChangesAsync();
            return queueId;
        }

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

        public async Task<int> GetQueueIdByEntryAsync(int entryId)
        {
            return await context.QueueEntries
                .Where(e => e.Id == entryId)
                .Select(e => e.QueueId)
                .FirstOrDefaultAsync();
        }
    }
}
