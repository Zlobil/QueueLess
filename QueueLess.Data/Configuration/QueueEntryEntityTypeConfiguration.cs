using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueLess.Data.Models;

namespace QueueLess.Data.Configuration
{
    public class QueueEntryEntityTypeConfiguration : IEntityTypeConfiguration<QueueEntry>
    {
        public void Configure(EntityTypeBuilder<QueueEntry> builder)
        {
            builder.HasData(
                new QueueEntry
                {
                    Id = 1,
                    ClientName = "Ivan Petrov",
                    JoinedOn = new DateTime(2026, 1, 15, 9, 0, 0),
                    QueueId = 1
                },
                new QueueEntry
                {
                    Id = 2,
                    ClientName = "Georgi Dimitrov",
                    JoinedOn = new DateTime(2026, 1, 15, 9, 30, 0),
                    QueueId = 1
                },
                new QueueEntry
                {
                    Id = 3,
                    ClientName = "Petar Kolev",
                    JoinedOn = new DateTime(2026, 1, 15, 10, 0, 0),
                    QueueId = 3
                },
                new QueueEntry
                {
                    Id = 4,
                    ClientName = "Maria Ivanova",
                    JoinedOn = new DateTime(2026, 1, 15, 10, 30, 0),
                    QueueId = 4
                },
                new QueueEntry
                {
                    Id = 5,
                    ClientName = "Nikolay Stoyanov",
                    JoinedOn = new DateTime(2026, 1, 15, 11, 0, 0),
                    QueueId = 5
                }
            );
        }
    }
}
