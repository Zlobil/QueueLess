namespace QueueLess.ViewModels
{
    public class QueueEntryViewModel
    {
        public int EntryId { get; set; }

        public int Position { get; set; }

        public string ClientName { get; set; } = null!;

        public DateTime JoinedOn { get; set; }
    }

}
