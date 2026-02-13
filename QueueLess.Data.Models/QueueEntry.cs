namespace QueueLess.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static QueueLess.GCommon.EntityValidation.QueueEntry;
    using QueueLess.Data.Models.Enums;

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
        public QueueEntryStatus Status { get; set; } = QueueEntryStatus.Waiting;

        [Required]
        [ForeignKey(nameof(Queue))]
        public int QueueId { get; set; }

        public Queue Queue { get; set; } = null!;
    }
}
