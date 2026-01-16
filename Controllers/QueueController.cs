using Microsoft.AspNetCore.Mvc;

namespace QueueLess.Controllers
{
    public class QueueController : Controller
    {
        public IActionResult Index()
        {
            return View();
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
            return View();
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
