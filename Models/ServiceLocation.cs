using QueueLess.Common;
using System.ComponentModel.DataAnnotations;

namespace QueueLess.Models
{
    public class ServiceLocation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(EntityValidation.ServiceLocation.NameMaxLength)]
        public string Name { get; set; } = null!;

        [MaxLength(EntityValidation.ServiceLocation.AddressMaxLength)]
        public string? Address { get; set; }

        [MaxLength(EntityValidation.ServiceLocation.PhoneNumberMaxLength)]
        public string? PhoneNumber { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
