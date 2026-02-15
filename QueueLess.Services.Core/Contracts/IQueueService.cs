namespace QueueLess.Services.Contracts
{
    using QueueLess.ViewModels.Queue;

    /// <summary>
    /// Service contract for managing queues, queue entries, and related operations.
    /// </summary>
    public interface IQueueService
    {
        /// <summary>
        /// Retrieves all queues owned by a specific user.
        /// </summary>
        /// <param name="ownerId">The ID of the user who owns the queues.</param>
        /// <returns>A collection of MyQueuesViewModel objects.</returns>
        Task<IEnumerable<MyQueuesViewModel>> GetAllQueuesAsync(string ownerId);

        /// <summary>
        /// Gets the view model for creating a new queue, including available service locations.
        /// </summary>
        /// <returns>A QueueCreateViewModel object with service locations.</returns>
        Task<QueueCreateViewModel> GetQueueCreateViewModelAsync();

        /// <summary>
        /// Checks whether a service location exists.
        /// </summary>
        /// <param name="id">Service location ID.</param>
        /// <returns>True if exists, false otherwise.</returns>
        Task<bool> ServiceLocationExistsAsync(int id);

        /// <summary>
        /// Creates a new queue for a given owner.
        /// </summary>
        /// <param name="model">Queue creation data.</param>
        /// <param name="ownerId">ID of the user creating the queue.</param>
        Task CreateQueueAsync(QueueCreateViewModel model, string ownerId);

        /// <summary>
        /// Retrieves detailed information about a specific queue.
        /// </summary>
        /// <param name="id">Queue ID.</param>
        /// <param name="ownerId">Owner's user ID.</param>
        /// <param name="tab">Optional active tab ("waiting" or "history").</param>
        /// <returns>A QueueDetailsViewModel or null if not found.</returns>
        Task<QueueDetailsViewModel?> GetQueueDetailsAsync(int id, string ownerId, string? tab);

        /// <summary>
        /// Marks a specific queue entry as served.
        /// </summary>
        /// <param name="entryId">Queue entry ID.</param>
        Task ServeEntryAsync(int entryId);

        /// <summary>
        /// Marks the next waiting entry in the queue as served.
        /// </summary>
        /// <param name="queueId">Queue ID.</param>
        Task ServeNextAsync(int queueId);

        /// <summary>
        /// Skips a specific queue entry.
        /// </summary>
        /// <param name="entryId">Queue entry ID.</param>
        Task SkipEntryAsync(int entryId);

        /// <summary>
        /// Cleans up historical entries of a queue based on specified criteria.
        /// </summary>
        /// <param name="model">Cleanup criteria (days limit).</param>
        Task CleanupHistoryAsync(QueueHistoryCleanupViewModel model);

        /// <summary>
        /// Deletes a specific history entry from a queue.
        /// </summary>
        /// <param name="entryId">Queue entry ID.</param>
        /// <returns>The ID of the associated queue.</returns>
        Task<int> DeleteHistoryEntryAsync(int entryId);

        /// <summary>
        /// Gets a queue for editing.
        /// </summary>
        /// <param name="id">Queue ID.</param>
        /// <param name="ownerId">Owner's user ID.</param>
        /// <returns>A QueueEditViewModel or null if not found.</returns>
        Task<QueueEditViewModel?> GetQueueForEditAsync(int id, string ownerId);

        /// <summary>
        /// Edits an existing queue.
        /// </summary>
        /// <param name="model">Queue edit data.</param>
        /// <param name="ownerId">Owner's user ID.</param>
        Task EditQueueAsync(QueueEditViewModel model, string ownerId);

        /// <summary>
        /// Gets a queue for deletion confirmation.
        /// </summary>
        /// <param name="id">Queue ID.</param>
        /// <param name="ownerId">Owner's user ID.</param>
        /// <returns>A QueueDeleteViewModel or null if not found.</returns>
        Task<QueueDeleteViewModel?> GetQueueForDeleteAsync(int id, string ownerId);

        /// <summary>
        /// Deletes a queue if it has no active entries.
        /// </summary>
        /// <param name="id">Queue ID.</param>
        /// <param name="ownerId">Owner's user ID.</param>
        /// <returns>True if deleted successfully, false otherwise.</returns>
        Task<bool> DeleteQueueAsync(int id, string ownerId);

        /// <summary>
        /// Retrieves all active queues (open for clients).
        /// </summary>
        /// <returns>Collection of QueueActiveViewModel objects.</returns>
        Task<IEnumerable<QueueActiveViewModel>> GetActiveQueuesAsync();

        /// <summary>
        /// Retrieves public view information for a queue.
        /// </summary>
        /// <param name="id">Queue ID.</param>
        /// <returns>A QueuePublicViewModel or null if not found.</returns>
        Task<QueuePublicViewModel?> GetPublicQueueAsync(int id);

        /// <summary>
        /// Retrieves the join page view model for a queue.
        /// </summary>
        /// <param name="id">Queue ID.</param>
        /// <returns>A QueueJoinViewModel or null if not found.</returns>
        Task<QueueJoinViewModel?> GetQueueJoinViewModelAsync(int id);

        /// <summary>
        /// Adds a client to the queue.
        /// </summary>
        /// <param name="model">QueueJoinViewModel with client information.</param>
        /// <returns>The ID of the newly created queue entry.</returns>
        Task<int> JoinQueueAsync(QueueJoinViewModel model);

        /// <summary>
        /// Retrieves the waiting view model for a specific queue entry.
        /// </summary>
        /// <param name="id">Queue ID.</param>
        /// <param name="entryId">Queue entry ID.</param>
        /// <returns>A QueueWaitingViewModel or null if not found or served/skipped.</returns>
        Task<QueueWaitingViewModel?> GetWaitingViewModelAsync(int id, int entryId);

        /// <summary>
        /// Retrieves the current waiting status for a specific queue entry.
        /// </summary>
        /// <param name="id">Queue ID.</param>
        /// <param name="entryId">Queue entry ID.</param>
        /// <returns>An anonymous object representing the current state.</returns>
        Task<object> GetWaitingStatusAsync(int id, int entryId);

        /// <summary>
        /// Gets the queue ID for a specific entry.
        /// </summary>
        /// <param name="entryId">Queue entry ID.</param>
        /// <returns>The queue ID.</returns>
        Task<int> GetQueueIdByEntryAsync(int entryId);
    }
}
