using QueueLess.Data.Models.Enums;

namespace QueueLess.ViewModels.Queue
{
    public class QueueDetailsHistoryViewModel
    {
        public int EntryId { get; set; }

        public string ClientName { get; set; } = null!;

        public QueueEntryStatus Status { get; set; }

        public DateTime JoinedOn { get; set; }
    }
}
