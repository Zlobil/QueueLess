namespace QueueLess.Services.Contracts
{
    using QueueLess.ViewModels.Queue;

    public interface IQueueService
    {
        Task<IEnumerable<MyQueuesViewModel>> GetAllQueuesAsync(string ownerId);

        Task<QueueCreateViewModel> GetQueueCreateViewModelAsync();
        Task<bool> ServiceLocationExistsAsync(int id);
        Task CreateQueueAsync(QueueCreateViewModel model, string ownerId);

        Task<QueueDetailsViewModel?> GetQueueDetailsAsync(int id, string ownerId, string? tab);

        Task ServeEntryAsync(int entryId);
        Task ServeNextAsync(int queueId);
        Task SkipEntryAsync(int entryId);

        Task CleanupHistoryAsync(QueueHistoryCleanupViewModel model);
        Task<int> DeleteHistoryEntryAsync(int entryId);

        Task<QueueEditViewModel?> GetQueueForEditAsync(int id, string ownerId);
        Task EditQueueAsync(QueueEditViewModel model, string ownerId);

        Task<QueueDeleteViewModel?> GetQueueForDeleteAsync(int id, string ownerId);
        Task<bool> DeleteQueueAsync(int id, string ownerId);

        Task<IEnumerable<QueueActiveViewModel>> GetActiveQueuesAsync();
        Task<QueuePublicViewModel?> GetPublicQueueAsync(int id);

        Task<QueueJoinViewModel?> GetQueueJoinViewModelAsync(int id);
        Task<int> JoinQueueAsync(QueueJoinViewModel model);

        Task<QueueWaitingViewModel?> GetWaitingViewModelAsync(int id, int entryId);
        Task<object> GetWaitingStatusAsync(int id, int entryId);

        Task<int> GetQueueIdByEntryAsync(int entryId);
    }
}
