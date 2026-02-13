namespace QueueLess.Services.Contracts
{
    using QueueLess.ViewModels.Queue;

    public interface IQueueService
    {
        Task<IEnumerable<MyQueuesViewModel>> GetAllQueuesAsync();
        Task<QueueCreateViewModel> GetQueueCreateViewModelAsync();
        Task<bool> ServiceLocationExistsAsync(int id);
        Task CreateQueueAsync(QueueCreateViewModel model);

        Task<QueueDetailsViewModel?> GetQueueDetailsAsync(int id, string? tab);

        Task ServeEntryAsync(int entryId);
        Task ServeNextAsync(int queueId);
        Task SkipEntryAsync(int entryId);

        Task CleanupHistoryAsync(QueueHistoryCleanupViewModel model);
        Task<int> DeleteHistoryEntryAsync(int entryId);

        Task<QueueEditViewModel?> GetQueueForEditAsync(int id);
        Task EditQueueAsync(QueueEditViewModel model);

        Task<QueueDeleteViewModel?> GetQueueForDeleteAsync(int id);
        Task<bool> DeleteQueueAsync(int id);

        Task<IEnumerable<QueueActiveViewModel>> GetActiveQueuesAsync();
        Task<QueuePublicViewModel?> GetPublicQueueAsync(int id);

        Task<QueueJoinViewModel?> GetQueueJoinViewModelAsync(int id);
        Task<int> JoinQueueAsync(QueueJoinViewModel model);

        Task<QueueWaitingViewModel?> GetWaitingViewModelAsync(int id, int entryId);
        Task<object> GetWaitingStatusAsync(int id, int entryId);

        Task<int> GetQueueIdByEntryAsync(int entryId);
    }
}
