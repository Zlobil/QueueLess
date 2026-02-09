using System.ComponentModel.DataAnnotations;

namespace QueueLess.ViewModels
{
    public class QueueJoinViewModel
    {
        public int QueueId { get; set; }

        public string? QueueName { get; set; }

        [Required]
        public string ClientName { get; set; } = null!;
    }
}
