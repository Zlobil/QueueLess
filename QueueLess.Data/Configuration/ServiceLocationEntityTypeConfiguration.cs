namespace QueueLess.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using QueueLess.Data.Models;

    public class ServiceLocationEntityTypeConfiguration : IEntityTypeConfiguration<ServiceLocation>
    {
        public void Configure(EntityTypeBuilder<ServiceLocation> builder)
        {
            builder.HasData(
                new ServiceLocation
                {
                    Id = 1,
                    Name = "AutoFix Garage",
                    Address = "12 Industrial Street, Sofia",
                    PhoneNumber = "+359888123456",
                    CreatedOn = new DateTime(2026, 1, 5)
                },
                new ServiceLocation
                {
                    Id = 2,
                    Name = "Downtown Barber Shop",
                    Address = "8 Main Blvd, Plovdiv",
                    PhoneNumber = "+359887222333",
                    CreatedOn = new DateTime(2026, 1, 6)
                },
                new ServiceLocation
                {
                    Id = 3,
                    Name = "Smile Dental Clinic",
                    Address = "25 Health St, Varna",
                    PhoneNumber = "+359889333444",
                    CreatedOn = new DateTime(2026, 1, 7)
                },
                new ServiceLocation
                {
                    Id = 4,
                    Name = "SmartFix Phone Repair",
                    Address = "14 Tech Ave, Sofia",
                    PhoneNumber = "+359886444555",
                    CreatedOn = new DateTime(2026, 1, 8)
                },
                new ServiceLocation
                {
                    Id = 5,
                    Name = "Municipal Services Office",
                    Address = "1 Freedom Sq, Burgas",
                    PhoneNumber = "+359885555666",
                    CreatedOn = new DateTime(2026, 1, 9)
                }
            );
        }
    }
}
