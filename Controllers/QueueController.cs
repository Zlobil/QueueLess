namespace QueueLess.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using QueueLess.Data;
    using QueueLess.Models;
    using QueueLess.ViewModels;

    public class QueueController : Controller
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
                IsOpen = model.IsOpen,
                CreatedOn = DateTime.UtcNow,
                ServiceLocationId = model.ServiceLocationId
            };

            context.Queues.Add(queue);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (id <= 0) return BadRequest();

            var queue = context.Queues
                .Include(q => q.QueueEntries)
                .FirstOrDefault(q => q.Id == id);

            if (queue == null) return NotFound();

            var entries = queue.QueueEntries
                .OrderBy(e => e.JoinedOn)
                .Select((e, index) => new QueueEntryViewModel
                {
                    EntryId = e.Id,
                    Position = index + 1,
                    ClientName = e.ClientName,
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
                WaitingCount = entries.Count,
                Entries = entries
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Edit(Queue input)
        {
            return Json(input);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var queue = context.Queues
                .Where(q => q.Id == id)
                .Select(q => new QueueDeleteViewModel
                {
                    Id = q.Id,
                    Name = q.Name,
                    Description = q.Description
                })
                .FirstOrDefault();
            
            if (queue == null) return NotFound();
            
            return View(queue);
        }

        [HttpPost]
        public IActionResult Delete(QueueDeleteViewModel model)
        {
            var queue = context.Queues
                .FirstOrDefault(q => q.Id == model.Id);

            if (queue == null) return NotFound();

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
            if (id <= 0) return BadRequest();

            var queue = context.Queues
                .Where(q => q.Id == id)
                .Select(q => new QueuePublicViewModel
                {
                    QueueId = q.Id,
                    Name = q.Name,
                    Description = q.Description, 
                    IsOpen = q.IsOpen,
                    AverageServiceTimeMinutes = q.AverageServiceTimeMinutes,
                    WaitingCount = q.QueueEntries.Count(),
                    EstimatedWaitingTimeMinutes = q.QueueEntries.Count() * q.AverageServiceTimeMinutes
                })
                .FirstOrDefault();

            if (queue == null) return NotFound();

            return View(queue);
        }

        [HttpGet]
        public IActionResult Join(int id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Join(Queue input)
        {
            return Json(input);
        }

        [HttpGet]
        public IActionResult Waiting(int id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Waiting(Queue input)
        {
            return Json(input);
        }

    }
}
