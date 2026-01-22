using Microsoft.AspNetCore.Mvc;
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
        
        public IActionResult Details()
        {
            return View();
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

        public IActionResult Public(int id = 1)
        {
            return View();
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
