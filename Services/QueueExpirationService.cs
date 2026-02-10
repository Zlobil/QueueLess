using Microsoft.EntityFrameworkCore;
using QueueLess.Data;
using QueueLess.Models.Enums;

namespace QueueLess.Services
{
    public class QueueExpirationService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public QueueExpirationService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ExpireEntries();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ExpireEntries()
        {
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var now = DateTime.UtcNow;
            var expired = context.QueueEntries
                .Include(e => e.Queue)
                .Where(e =>
                    e.Status == QueueEntryStatus.Waiting &&
                    e.JoinedOn.AddMinutes(e.Queue.MaxWaitMinutes) < now)
                .ToList();
            if (!expired.Any())
                return;

            foreach (var e in expired)
            {
                e.Status = QueueEntryStatus.Expired;
            }

            await context.SaveChangesAsync();
        }
    }
}
