namespace QueueLess.ViewModels.Queue
{
    public class MyQueuesViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsOpen { get; set; }

        public DateTime CreatedOn { get; set; }

        public int AverageServiceTimeMinutes { get; set; }
    }

}
