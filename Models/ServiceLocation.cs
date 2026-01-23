namespace QueueLess.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Common.EntityValidation.ServiceLocation;

    public class ServiceLocation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [MaxLength(AddressMaxLength)]
        public string? Address { get; set; }

        [MaxLength(PhoneNumberMaxLength)]
        public string? PhoneNumber { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
