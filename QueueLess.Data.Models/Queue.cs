namespace QueueLess.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static QueueLess.GCommon.EntityValidation.Queue;

    /// <summary>
    /// Represents a service queue created by a business user.
    /// Contains information about service type, duration, status, and owner.
    /// </summary>
    public class Queue
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of the queue.
        /// </summary>
        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Optional description of the service queue.
        /// </summary>
        [MaxLength(DescriptionMaxLength)]
        public string? Description { get; set; }

        /// <summary>
        /// Average service time in minutes.
        /// </summary>
        [Required]
        [Range(AverageServiceTimeMin, AverageServiceTimeMax)]
        public int AverageServiceTimeMinutes { get; set; }

        /// <summary>
        /// Maximum waiting time allowed.
        /// </summary>
        [Required]
        [Range(MaxWaitMinutesMin, MaxWaitMinutesMax)]
        public int MaxWaitMinutes { get; set; }

        /// <summary>
        /// Indicates whether the queue is currently open.
        /// </summary>
        [Required]
        public bool IsOpen { get; set; }

        /// <summary>
        /// The date and time when the queue was created.
        /// </summary>
        [Required]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Foreign key to the service location.
        /// </summary>
        [Required]
        [ForeignKey(nameof(ServiceLocation))]
        public int ServiceLocationId { get; set; }

        /// <summary>
        /// Navigation property to the service location.
        /// </summary>
        public ServiceLocation ServiceLocation { get; set; } = null!;

        /// <summary>
        /// Foreign key to the owner user.
        /// </summary>
        [Required]
        public string OwnerId { get; set; } = null!;

        /// <summary>
        /// Navigation property to the owner IdentityUser.
        /// </summary>
        [ForeignKey(nameof(OwnerId))]
        public IdentityUser Owner { get; set; } = null!;

        /// <summary>
        /// Collection of queue entries (clients in the queue).
        /// </summary>
        public ICollection<QueueEntry> QueueEntries { get; set; } = new HashSet<QueueEntry>();
    }
}
