namespace QueueLess.ViewModels
{
    public class QueueActiveViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int AverageServiceTimeMinutes { get; set; }

        public bool IsOpen { get; set; }
    }
}
