namespace QueueLess.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using QueueLess.Data;
    using QueueLess.Models;
    using QueueLess.Models.Enums;
    using QueueLess.ViewModels;
    using System;

    public class QueueController : BaseController
    {
        private readonly ApplicationDbContext context;

        public QueueController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var queues = context.Queues
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
            .ToList();

            return View(queues);
        }

        // Business side

        [HttpGet]
        public IActionResult Create()
        {
            var model = new QueueCreateViewModel
            {
                ServiceLocations = context.ServiceLocations
                .OrderBy(sl => sl.Name)
                .Select(sl => new ServiceLocationSelectViewModel
                {
                    Id = sl.Id,
                    Name = sl.Name
                })
                .ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(QueueCreateViewModel model)
        {
            var serviceLocationExists = context.ServiceLocations
                .Any(sl => sl.Id == model.ServiceLocationId);
            if (!serviceLocationExists)
            {
                ModelState.AddModelError(
                    nameof(model.ServiceLocationId),
                    "Invalid service location."
                );
            }

            if (!ModelState.IsValid)
            {
                model.ServiceLocations = context.ServiceLocations
                    .Select(sl => new ServiceLocationSelectViewModel
                    {
                        Id = sl.Id,
                        Name = sl.Name
                    })
                    .ToList();

                return View(model);
            }

            var queue = new Queue
            {
                Name = model.Name,
                Description = model.Description,
                AverageServiceTimeMinutes = model.AverageServiceTimeMinutes,
                MaxWaitMinutes = model.MaxWaitMinutes,
                IsOpen = model.IsOpen,
                CreatedOn = DateTime.UtcNow,
                ServiceLocationId = model.ServiceLocationId
            };

            context.Queues.Add(queue);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(int id, string? tab = "waiting")
        {
            if (id <= 0)
                return BadRequest();

            var queue = context.Queues
                .Include(q => q.QueueEntries)
                .FirstOrDefault(q => q.Id == id);
            if (queue == null)
                return NotFound();
            
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

            var model = new QueueDetailsViewModel
            {
                QueueId = queue.Id,
                Name = queue.Name,
                Description = queue.Description,
                IsOpen = queue.IsOpen,
                AverageServiceTimeMinutes = queue.AverageServiceTimeMinutes,
                CreatedOn = queue.CreatedOn,
                Entries = waiting,
                History = history,
                ActiveTab = tab
            };

            return View(model);
        }

        private async Task Serving(int id)
        {
            var queue = await context.Queues
                .Include(q => q.QueueEntries)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (queue == null)
                return;

            var serving = queue.QueueEntries
                .FirstOrDefault(e => e.Status == QueueEntryStatus.Serving);
            if (serving != null)
                return;

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

        [HttpPost]
        public async Task<IActionResult> Serve(int id)
        {
            if (id <= 0)
                return BadRequest();

            var entry = context.QueueEntries.FirstOrDefault(e => e.Id == id);
            if (entry == null)
                return NotFound();

            entry.Status = QueueEntryStatus.Served;
            await context.SaveChangesAsync();

            await Serving(entry.QueueId);

            return RedirectToAction("Details", new { id = entry.QueueId });
        }

        [HttpPost]
        public async Task<IActionResult> ServeNext(int id)
        {
            if (id <= 0)
                return BadRequest();

            var queue = context.Queues
                .Include(q => q.QueueEntries)
                .FirstOrDefault(q => q.Id == id);
            if (queue == null)
                return NotFound();
            
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

            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Skip(int id)
        {
            if (id <= 0)
                return BadRequest();

            var entry = context.QueueEntries.FirstOrDefault(e => e.Id == id);
            if (entry == null)
                return NotFound();

            entry.Status = QueueEntryStatus.Skipped;
            await context.SaveChangesAsync();

            await Serving(entry.QueueId);

            return RedirectToAction("Details", new { id = entry.QueueId });
        }

        [HttpPost]
        public async Task<IActionResult> CleanupHistory(QueueHistoryCleanupViewModel model)
        {
            if (model.QueueId <= 0)
                return BadRequest();

            var query = context.QueueEntries
                .Where(e => e.QueueId == model.QueueId && e.Status != QueueEntryStatus.Waiting);
            if (model.Days != -1)
            {
                var cutoff = DateTime.UtcNow.AddDays(-model.Days);
                query = query.Where(e => e.JoinedOn < cutoff);
            }

            context.QueueEntries.RemoveRange(query);
            await context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = model.QueueId, tab = "history" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHistoryEntry(int id)
        {
            if (id <= 0)
                return BadRequest();

            var entry = await context.QueueEntries.FindAsync(id);
            if (entry == null)
                return NotFound();

            context.QueueEntries.Remove(entry);
            await context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = entry.QueueId, tab = "history" });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var model = context.Queues
                .Where(q => q.Id == id)
                .Select(q => new QueueEditViewModel
                {
                    Id = q.Id,
                    Name = q.Name,
                    Description = q.Description,
                    AverageServiceTimeMinutes = q.AverageServiceTimeMinutes,
                    MaxWaitMinutes = q.MaxWaitMinutes,
                    IsOpen = q.IsOpen
                })
                .FirstOrDefault();
            if (model == null)
                return NotFound();
            
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(QueueEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var queue = context.Queues
                .FirstOrDefault(q => q.Id == model.Id);
            if (queue == null)
                return NotFound();

            queue.Name = model.Name;
            queue.Description = model.Description;
            queue.AverageServiceTimeMinutes = model.AverageServiceTimeMinutes;
            queue.MaxWaitMinutes = model.MaxWaitMinutes;
            queue.IsOpen = model.IsOpen;

            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
                return BadRequest();

            var queue = context.Queues
                .Where(q => q.Id == id)
                .Select(q => new QueueDeleteViewModel
                {
                    Id = q.Id,
                    Name = q.Name,
                    Description = q.Description
                })
                .FirstOrDefault();
            if (queue == null)
                return NotFound();
            
            return View(queue);
        }

        [HttpPost]
        public IActionResult Delete(QueueDeleteViewModel model)
        {
            var queue = context.Queues
                .FirstOrDefault(q => q.Id == model.Id);
            if (queue == null)
                return NotFound();

            bool hasEntries = context.QueueEntries
                .Any(e => e.QueueId == queue.Id);
            if (hasEntries)
            {
                ModelState.AddModelError(string.Empty, "Queue cannot be deleted because it has active entries.");
                return View(model);
            }

            context.Queues.Remove(queue);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // Client side

        [HttpGet]
        public IActionResult Active()
        {
            var queues = context.Queues
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
                .ToList();

            return View(queues);
        }

        [HttpGet]
        public IActionResult Public(int id)
        {
            if (id <= 0)
                return BadRequest();

            var queue = context.Queues
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
                        q.QueueEntries.Count(e => e.Status == QueueEntryStatus.Waiting) * q.AverageServiceTimeMinutes
                })
                .FirstOrDefault();
            if (queue == null)
                return NotFound();

            return View(queue);
        }

        [HttpGet]
        public IActionResult Join(int id)
        {
            if (id <= 0)
                return BadRequest();

            var queue = context.Queues.FirstOrDefault(q => q.Id == id);
            if (queue == null)
                return NotFound();

            var model = new QueueJoinViewModel
            {
                QueueId = queue.Id,
                QueueName = queue.Name
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Join(QueueJoinViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var entry = new QueueEntry
            {
                QueueId = model.QueueId,
                ClientName = model.ClientName,
                JoinedOn = DateTime.UtcNow,
                Status = QueueEntryStatus.Waiting
            };
            context.QueueEntries.Add(entry);
            await context.SaveChangesAsync();

            await Serving(model.QueueId);

            return RedirectToAction("Waiting", new { id = model.QueueId, entryId = entry.Id });
        }

        [HttpGet("Queue/Waiting/{id}/{entryId}")]
        public async Task<IActionResult> Waiting(int id, int entryId)
        {
            if (id <= 0 || entryId <= 0)
                return BadRequest();

            await Serving(id);

            var queue = context.Queues
                .Include(q => q.QueueEntries)
                .FirstOrDefault(q => q.Id == id);
            if (queue == null)
                return NotFound();

            var entry = queue.QueueEntries.FirstOrDefault(e => e.Id == entryId);
            if (entry == null)
                return NotFound();

            if (entry.Status != QueueEntryStatus.Waiting && 
                entry.Status != QueueEntryStatus.Serving)
                return RedirectToAction("WaitingResult", new { id = queue.Id, entryId = entry.Id });

            var ordered = queue.QueueEntries
                .Where(e => e.Status == QueueEntryStatus.Waiting ||
                            e.Status == QueueEntryStatus.Serving)
                .OrderBy(e => e.JoinedOn)
                .ToList();

            var position = ordered.FindIndex(e => e.Id == entry.Id) + 1;
            var ahead = Math.Max(position - 1, 0);
            var estimated = ahead * queue.AverageServiceTimeMinutes;

            var model = new QueueWaitingViewModel
            {
                QueueId = queue.Id,
                EntryId = entry.Id,
                QueueName = queue.Name,
                Position = position,
                PeopleAhead = ahead,
                EstimatedWaitMinutes = estimated
            };

            return View(model);
        }

        [HttpGet("Queue/WaitingStatus/{id}/{entryId}")]
        public IActionResult WaitingStatus(int id, int entryId)
        {
            if (id <= 0 || entryId <= 0)
                return BadRequest();

            var queue = context.Queues
                .Include(q => q.QueueEntries)
                .FirstOrDefault(q => q.Id == id);
            if (queue == null)
                return Json(new { state = "queue_removed" });

            var entry = queue.QueueEntries
                .FirstOrDefault(e => e.Id == entryId);
            if (entry == null)
                return Json(new { state = "removed" });

            if (entry.Status == QueueEntryStatus.Serving)
                return Json(new { state = "your_turn", position = 1, ahead = 0, estimated = 0 });

            if (entry.Status != QueueEntryStatus.Waiting)
                return Json(new { state = entry.Status.ToString().ToLower() });

            var active = queue.QueueEntries
                .Where(e => e.Status == QueueEntryStatus.Waiting ||
                            e.Status == QueueEntryStatus.Serving)
                .OrderBy(e => e.JoinedOn)
                .ToList();

            var position = active.FindIndex(e => e.Id == entry.Id) + 1;
            var ahead = Math.Max(position - 1, 0);
            var estimated = ahead * queue.AverageServiceTimeMinutes;

            return Json(new { state = "waiting", position, ahead, estimated });
        }

        [HttpGet]
        public IActionResult WaitingResult(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
                return RedirectToAction("Index", "Home");

            var model = new QueueWaitingResultViewModel
            {
                State = state
            };

            return View(model);
        }

    }
}
