namespace QueueLess.ViewModels
{
    public class PublicQueueViewModel
    {
        public int QueueId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsOpen { get; set; }

        public int AverageServiceTimeMinutes { get; set; }

        public int WaitingCount { get; set; }

        public int EstimatedWaitingTimeMinutes { get; set; }
    }
}
