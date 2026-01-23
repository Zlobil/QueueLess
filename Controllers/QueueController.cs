using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueueLess.Data;
using QueueLess.ViewModels;

namespace QueueLess.Controllers
{
    public class QueueController : Controller
    {
        private readonly ApplicationDbContext context;

        public QueueController(ApplicationDbContext context)
        {
            this.context = context;
        }

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

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
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

        public IActionResult Edit()
        {
            return View();
        }
        
        public IActionResult Delete()
        {
            return View();
        }
        
        // Client side

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

        public IActionResult Public(int id)
        {
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


        public IActionResult Join(int id = 1)
        {
            return View();
        }

        public IActionResult Waiting()
        {
            return View();
        }

    }
}
