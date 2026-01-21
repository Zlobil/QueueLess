using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueLess.Models;

namespace QueueLess.Data.Configuration
{
    public class QueueEntityTypeConfiguration : IEntityTypeConfiguration<Queue>
    {
        public void Configure(EntityTypeBuilder<Queue> builder)
        {
            builder.HasData(
                new Queue
                {
                    Id = 1,
                    Name = "Oil Change Service",
                    Description = "Engine oil and oil filter replacement.",
                    AverageServiceTimeMinutes = 30,
                    IsOpen = true,
                    CreatedOn = new DateTime(2026, 1, 10),
                    ServiceLocationId = 1
                },
                new Queue
                {
                    Id = 2,
                    Name = "Brake Inspection",
                    Description = "Brake system inspection and diagnostics.",
                    AverageServiceTimeMinutes = 45,
                    IsOpen = true,
                    CreatedOn = new DateTime(2026, 1, 11),
                    ServiceLocationId = 1
                },

                new Queue
                {
                    Id = 3,
                    Name = "Men's Haircut",
                    Description = "Classic men's haircut and styling.",
                    AverageServiceTimeMinutes = 25,
                    IsOpen = true,
                    CreatedOn = new DateTime(2026, 1, 12),
                    ServiceLocationId = 2
                },

                new Queue
                {
                    Id = 4,
                    Name = "Dental Check-up",
                    Description = "Routine dental examination.",
                    AverageServiceTimeMinutes = 20,
                    IsOpen = true,
                    CreatedOn = new DateTime(2026, 1, 13),
                    ServiceLocationId = 3
                },

                new Queue
                {
                    Id = 5,
                    Name = "Phone Screen Replacement",
                    Description = "Smartphone screen replacement service.",
                    AverageServiceTimeMinutes = 40,
                    IsOpen = true,
                    CreatedOn = new DateTime(2026, 1, 14),
                    ServiceLocationId = 4
                }
            );
        }
    }
}
