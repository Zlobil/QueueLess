namespace QueueLess.Services
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using QueueLess.Data;
    using QueueLess.Data.Models.Enums;

    /// <summary>
    /// Background service that automatically marks queue entries as expired
    /// when their maximum waiting time has elapsed.
    /// Runs every minute and updates all relevant entries in the database.
    /// </summary>
    public class QueueExpirationService(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ExpireEntries(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ExpireEntries(CancellationToken cancellationToken)
        {
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var now = DateTime.UtcNow;

            var expired = await context.QueueEntries
                .Include(e => e.Queue)
                .Where(e => (e.Status == QueueEntryStatus.Waiting ||
                            e.Status == QueueEntryStatus.Serving) &&
                            e.Queue != null &&
                            e.Queue.MaxWaitMinutes > 0 &&
                            e.JoinedOn.AddMinutes(e.Queue.MaxWaitMinutes) < now)
                .ToListAsync(cancellationToken);
            if (!expired.Any())
                return;

            foreach (var e in expired)
            {
                e.Status = QueueEntryStatus.Expired;
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
