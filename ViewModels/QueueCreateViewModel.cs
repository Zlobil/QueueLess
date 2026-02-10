namespace QueueLess.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using static Common.EntityValidation.Queue;

    public class QueueCreateViewModel
    {
        [Required]
        [Display(Name = "Service Location")]
        public int ServiceLocationId { get; set; }

        public IEnumerable<ServiceLocationSelectViewModel> ServiceLocations { get; set; }
            = new List<ServiceLocationSelectViewModel>();

        [Required]
        [MaxLength(NameMaxLength)]
        [Display(Name = "Queue Name")]
        public string Name { get; set; } = null!;

        [MaxLength(DescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        [Range(AverageServiceTimeMin, AverageServiceTimeMax)]
        [Display(Name = "Average Service Time (minutes)")]
        public int AverageServiceTimeMinutes { get; set; }

        [Required]
        [Range(MaxWaitMinutesMin, MaxWaitMinutesMax)]
        [Display(Name = "Max wait time (minutes)")]
        public int MaxWaitMinutes { get; set; }

        [Display(Name = "Queue is open")]
        public bool IsOpen { get; set; } = true;
    }
}
