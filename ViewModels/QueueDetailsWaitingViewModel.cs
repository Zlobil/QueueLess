using QueueLess.Models.Enums;

namespace QueueLess.ViewModels
{
    public class QueueDetailsWaitingViewModel
    {
        public int EntryId { get; set; }

        public int Position { get; set; }

        public string ClientName { get; set; } = null!;

        public QueueEntryStatus Status { get; set; }

        public DateTime JoinedOn { get; set; }
    }

}
