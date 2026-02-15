namespace QueueLess.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static QueueLess.GCommon.EntityValidation.ServiceLocation;

    /// <summary>
    /// Represents a physical location where services are provided.
    /// </summary>
    public class ServiceLocation
    {
        /// <summary>
        /// Primary key of the service location.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of the service location.
        /// </summary>
        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Name of the service location.
        /// </summary>
        [MaxLength(AddressMaxLength)]
        public string? Address { get; set; }

        /// <summary>
        /// Optional contact phone number.
        /// </summary>
        [MaxLength(PhoneNumberMaxLength)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Date and time when the service location was created.
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
