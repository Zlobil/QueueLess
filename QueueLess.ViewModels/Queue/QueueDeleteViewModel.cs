namespace QueueLess.ViewModels.Queue
{
    using System.ComponentModel.DataAnnotations;

    public class QueueDeleteViewModel
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
