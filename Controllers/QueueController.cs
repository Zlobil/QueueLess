namespace QueueLess.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using QueueLess.Services.Contracts;
    using QueueLess.ViewModels;

    public class QueueController : BaseController
    {

        private readonly IQueueService queueService;

        public QueueController(IQueueService queueService)
        {
            this.queueService = queueService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var queues = await queueService.GetAllQueuesAsync();
            return View(queues);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await queueService.GetQueueCreateViewModelAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(QueueCreateViewModel model)
        {
            if (!await queueService.ServiceLocationExistsAsync(model.ServiceLocationId))
            {
                ModelState.AddModelError(nameof(model.ServiceLocationId), "Invalid service location.");
            }

            if (!ModelState.IsValid)
            {
                model = await queueService.GetQueueCreateViewModelAsync();
                return View(model);
            }

            await queueService.CreateQueueAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, string? tab = "waiting")
        {
            var model = await queueService.GetQueueDetailsAsync(id, tab);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Serve(int id)
        {
            await queueService.ServeEntryAsync(id);
            var queueId = await queueService.GetQueueIdByEntryAsync(id);
            return RedirectToAction("Details", new { id = queueId });
        }

        [HttpPost]
        public async Task<IActionResult> ServeNext(int id)
        {
            await queueService.ServeNextAsync(id);
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Skip(int id)
        {
            await queueService.SkipEntryAsync(id);
            var queueId = await queueService.GetQueueIdByEntryAsync(id);
            return RedirectToAction("Details", new { id = queueId });
        }

        [HttpPost]
        public async Task<IActionResult> CleanupHistory(QueueHistoryCleanupViewModel model)
        {
            await queueService.CleanupHistoryAsync(model);
            return RedirectToAction("Details", new { id = model.QueueId, tab = "history" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHistoryEntry(int id)
        {
            var queueId = await queueService.DeleteHistoryEntryAsync(id);
            return RedirectToAction("Details", new { id = queueId, tab = "history" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await queueService.GetQueueForEditAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(QueueEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await queueService.EditQueueAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await queueService.GetQueueForDeleteAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(QueueDeleteViewModel model)
        {
            bool deleted = await queueService.DeleteQueueAsync(model.Id);
            if (!deleted)
            {
                ModelState.AddModelError(string.Empty, "Queue cannot be deleted because it has active entries.");
                return View(model);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Active()
        {
            var queues = await queueService.GetActiveQueuesAsync();
            return View(queues);
        }

        [HttpGet]
        public async Task<IActionResult> Public(int id)
        {
            var model = await queueService.GetPublicQueueAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Join(int id)
        {
            var model = await queueService.GetQueueJoinViewModelAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Join(QueueJoinViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            int entryId = await queueService.JoinQueueAsync(model);
            return RedirectToAction("Waiting", new { id = model.QueueId, entryId });
        }

        [HttpGet("Queue/Waiting/{id}/{entryId}")]
        public async Task<IActionResult> Waiting(int id, int entryId)
        {
            var model = await queueService.GetWaitingViewModelAsync(id, entryId);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpGet("Queue/WaitingStatus/{id}/{entryId}")]
        public async Task<IActionResult> WaitingStatus(int id, int entryId)
        {
            return Json(await queueService.GetWaitingStatusAsync(id, entryId));
        }

        [HttpGet]
        public IActionResult WaitingResult(string state)
        {
            var model = new QueueWaitingResultViewModel { State = state };
            return View(model);
        }
    }
}
