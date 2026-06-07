using Betrieb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Betrieb.Infrastructure.Persistence.Configurations;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.ClockIn).IsRequired();

        builder.HasIndex(t => new { t.UserId, t.ClockOut });
    }
}
