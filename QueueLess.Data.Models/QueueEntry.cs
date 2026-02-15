namespace QueueLess.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static QueueLess.GCommon.EntityValidation.QueueEntry;
    using QueueLess.Data.Models.Enums;

    /// <summary>
    /// Represents an entry (client) in a service queue.
    /// Tracks client name, join time, status, and associated queue.
    /// </summary>
    public class QueueEntry
    {
        /// <summary>
        /// Primary key of the queue entry.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of the client who joined the queue.
        /// </summary>
        [Required]
        [MaxLength(ClientNameMaxLength)]
        public string ClientName { get; set; } = null!;

        /// <summary>
        /// Date and time when the client joined the queue.
        /// </summary>
        [Required]
        public DateTime JoinedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Current status of the queue entry (Waiting, Serving, Served, Skipped, Expired).
        /// </summary>
        [Required]
        public QueueEntryStatus Status { get; set; } = QueueEntryStatus.Waiting;

        /// <summary>
        /// Foreign key to the queue this entry belongs to.
        /// </summary>
        [Required]
        [ForeignKey(nameof(Queue))]
        public int QueueId { get; set; }

        /// <summary>
        /// Navigation property to the queue.
        /// </summary>
        public Queue Queue { get; set; } = null!;
    }
}
