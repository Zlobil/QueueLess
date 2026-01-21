using Microsoft.EntityFrameworkCore;
using QueueLess.Models;

namespace QueueLess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Queue> Queues { get; set; }
        public DbSet<ServiceLocation> ServiceLocations { get; set; }
        public DbSet<QueueEntry> QueueEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            /*
            modelBuilder.Entity<ServiceLocation>()
                .HasMany<Queue>()
                .WithOne(q => q.ServiceLocation)
                .HasForeignKey(q => q.ServiceLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Queue>()
                .HasMany<QueueEntry>()
                .WithOne()
                .HasForeignKey(qe => qe.QueueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Queue>()
                .Property(q => q.IsOpen)
                .HasDefaultValue(true);

            modelBuilder.Entity<Queue>()
                .Property(q => q.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");
            */
        }

    }
}
