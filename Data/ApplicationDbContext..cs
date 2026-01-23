using Microsoft.EntityFrameworkCore;
using QueueLess.Models;

namespace QueueLess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Queue> Queues { get; set; }
        public DbSet<ServiceLocation> ServiceLocations { get; set; }
        public DbSet<QueueEntry> QueueEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

    }
}
