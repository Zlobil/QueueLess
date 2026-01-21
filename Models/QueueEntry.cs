using QueueLess.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueLess.Models
{
    public class QueueEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(EntityValidation.QueueEntry.ClientNameMaxLength)]
        public string ClientName { get; set; } = null!;

        [Required]
        public DateTime JoinedOn { get; set; } = DateTime.UtcNow;

        [Required]
        [ForeignKey(nameof(Queue))]
        public int QueueId { get; set; }
    }
}
