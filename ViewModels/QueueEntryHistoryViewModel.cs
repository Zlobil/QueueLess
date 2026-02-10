using QueueLess.Models.Enums;

namespace QueueLess.ViewModels
{
    public class QueueEntryHistoryViewModel
    {
        public int EntryId { get; set; }

        public string ClientName { get; set; } = null!;

        public QueueEntryStatus Status { get; set; }

        public DateTime JoinedOn { get; set; }
    }
}
