namespace QueueLess.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static Common.EntityValidation.QueueEntry;

    public class QueueEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ClientNameMaxLength)]
        public string ClientName { get; set; } = null!;

        [Required]
        public DateTime JoinedOn { get; set; } = DateTime.UtcNow;

        [Required]
        [ForeignKey(nameof(Queue))]
        public int QueueId { get; set; }
    }
}
