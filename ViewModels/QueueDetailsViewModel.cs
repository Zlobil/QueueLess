namespace QueueLess.ViewModels
{
    public class QueueDetailsViewModel
    {
        public int QueueId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsOpen { get; set; }

        public int AverageServiceTimeMinutes { get; set; }

        public DateTime CreatedOn { get; set; }

        public int WaitingCount { get; set; }

        public List<QueueEntryViewModel> Entries { get; set; } = new();
        
        public List<QueueEntryHistoryViewModel> History { get; set; } = new();

        public string ActiveTab { get; set; } = "waiting";
    }
}
