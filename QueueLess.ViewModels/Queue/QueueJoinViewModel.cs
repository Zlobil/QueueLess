using System.ComponentModel.DataAnnotations;

namespace QueueLess.ViewModels.Queue
{
    public class QueueJoinViewModel
    {
        public int QueueId { get; set; }

        public string? QueueName { get; set; }

        [Required]
        public string ClientName { get; set; } = null!;
    }
}
