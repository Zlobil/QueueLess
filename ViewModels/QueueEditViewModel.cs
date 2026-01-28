namespace QueueLess.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using static Common.EntityValidation.Queue;

    public class QueueEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [MaxLength(DescriptionMaxLength)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required]
        [Range(AverageServiceTimeMin, AverageServiceTimeMax)]
        [Display(Name = "Average Service Time (minutes)")]
        public int AverageServiceTimeMinutes { get; set; }

        [Display(Name = "Queue is open")]
        public bool IsOpen { get; set; }
    }
}
