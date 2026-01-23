namespace QueueLess.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static Common.EntityValidation.Queue;

    public class Queue
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [MaxLength(DescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        [Range(AverageServiceTimeMin, AverageServiceTimeMax)]
        public int AverageServiceTimeMinutes { get; set; }

        [Required]
        public bool IsOpen { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        [ForeignKey(nameof(ServiceLocation))]
        public int ServiceLocationId { get; set; }

        public ServiceLocation ServiceLocation { get; set; } = null!;

        public ICollection<QueueEntry> QueueEntries { get; set; } = new HashSet<QueueEntry>();
    }
}
