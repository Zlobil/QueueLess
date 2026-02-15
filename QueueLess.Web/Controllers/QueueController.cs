namespace QueueLess.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using QueueLess.Services.Contracts;
    using QueueLess.ViewModels.Queue;

    /// <summary>
    /// Controller responsible for managing queues. 
    /// Handles actions accessible only by authenticated users as well as public queue actions.
    /// </summary>
    public class QueueController : BaseController
    {

        private readonly IQueueService queueService;

        public QueueController(IQueueService queueService)
        {
            this.queueService = queueService;
        }

        /// <summary>
        /// Displays all queues owned by the current user.
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId = GetUserId()!;
            var queues = await queueService.GetAllQueuesAsync(userId);
            return View(queues);
        }

        /// <summary>
        /// Displays the queue creation form.
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await queueService.GetQueueCreateViewModelAsync();
            return View(model);
        }

        /// <summary>
        /// Handles the POST request to create a new queue.
        /// </summary>
        [Authorize]
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

            string userId = GetUserId()!;
            await queueService.CreateQueueAsync(model, userId);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays detailed view of a queue, including waiting clients and history tabs.
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int id, string? tab = "waiting")
        {
            string userId = GetUserId()!;
            var model = await queueService.GetQueueDetailsAsync(id, userId, tab);
            if (model == null) return NotFound();
            return View(model);
        }

        /// <summary>
        /// Marks a specific queue entry as served.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Serve(int id)
        {
            await queueService.ServeEntryAsync(id);
            var queueId = await queueService.GetQueueIdByEntryAsync(id);
            return RedirectToAction("Details", new { id = queueId });
        }

        /// <summary>
        /// Serves the next client in the queue.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ServeNext(int id)
        {
            await queueService.ServeNextAsync(id);
            return RedirectToAction("Details", new { id });
        }

        /// <summary>
        /// Skips a specific queue entry.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Skip(int id)
        {
            await queueService.SkipEntryAsync(id);
            var queueId = await queueService.GetQueueIdByEntryAsync(id);
            return RedirectToAction("Details", new { id = queueId });
        }

        /// <summary>
        /// Cleans up history entries of a queue based on the specified model.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CleanupHistory(QueueHistoryCleanupViewModel model)
        {
            await queueService.CleanupHistoryAsync(model);
            return RedirectToAction("Details", new { id = model.QueueId, tab = "history" });
        }

        /// <summary>
        /// Deletes a single history entry from a queue.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteHistoryEntry(int id)
        {
            var queueId = await queueService.DeleteHistoryEntryAsync(id);
            return RedirectToAction("Details", new { id = queueId, tab = "history" });
        }

        /// <summary>
        /// Displays the edit form for a queue.
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            string userId = GetUserId()!;
            var model = await queueService.GetQueueForEditAsync(id, userId);
            if (model == null) return NotFound();
            return View(model);
        }

        /// <summary>
        /// Handles the POST request to update a queue.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(QueueEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            string userId = GetUserId()!;
            await queueService.EditQueueAsync(model, userId);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the delete confirmation page for a queue.
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            string userId = GetUserId()!;
            var model = await queueService.GetQueueForDeleteAsync(id, userId);
            if (model == null) return NotFound();
            return View(model);
        }

        /// <summary>
        /// Handles the POST request to delete a queue.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(QueueDeleteViewModel model)
        {
            string userId = GetUserId()!;
            bool deleted = await queueService.DeleteQueueAsync(model.Id, userId);
            if (!deleted)
            {
                ModelState.AddModelError(string.Empty, "Queue cannot be deleted because it has active entries.");
                return View(model);
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays all active queues (public view).
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Active()
        {
            var queues = await queueService.GetActiveQueuesAsync();
            return View(queues);
        }

        /// <summary>
        /// Displays public details of a specific queue.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Public(int id)
        {
            var model = await queueService.GetPublicQueueAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        /// <summary>
        /// Displays the join queue form.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Join(int id)
        {
            var model = await queueService.GetQueueJoinViewModelAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        /// <summary>
        /// Handles the POST request to join a queue.
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Join(QueueJoinViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            int entryId = await queueService.JoinQueueAsync(model);
            return RedirectToAction("Waiting", new { id = model.QueueId, entryId });
        }

        /// <summary>
        /// Displays the waiting page for a client in a queue.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("Queue/Waiting/{id}/{entryId}")]
        public async Task<IActionResult> Waiting(int id, int entryId)
        {
            var model = await queueService.GetWaitingViewModelAsync(id, entryId);
            if (model == null) return NotFound();
            return View(model);
        }

        /// <summary>
        /// Returns the JSON status of a queue entry for real-time updates.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("Queue/WaitingStatus/{id}/{entryId}")]
        public async Task<IActionResult> WaitingStatus(int id, int entryId)
        {
            return Json(await queueService.GetWaitingStatusAsync(id, entryId));
        }

        /// <summary>
        /// Displays the waiting result page based on a given state.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult WaitingResult(string state)
        {
            var model = new QueueWaitingResultViewModel { State = state };
            return View(model);
        }
    }
}
