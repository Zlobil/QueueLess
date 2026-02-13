namespace QueueLess.ViewModels.Queue
{
    using QueueLess.Data.Models.Enums;

    public class QueueDetailsWaitingViewModel
    {
        public int EntryId { get; set; }

        public int Position { get; set; }

        public string ClientName { get; set; } = null!;

        public QueueEntryStatus Status { get; set; }

        public DateTime JoinedOn { get; set; }
    }

}
