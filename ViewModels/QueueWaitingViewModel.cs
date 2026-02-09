namespace QueueLess.ViewModels
{
    public class QueueWaitingViewModel
    {
        public string QueueName { get; set; } = null!;

        public int Position { get; set; }

        public int PeopleAhead { get; set; }

        public int EstimatedWaitMinutes { get; set; }
    }

}
