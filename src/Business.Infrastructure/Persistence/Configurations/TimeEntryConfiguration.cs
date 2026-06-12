using Business.Domain.Entities;
using Business.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Business.Infrastructure.Persistence.Configurations;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.ClockIn).IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(TimeEntryStatus.Approved);

        builder.Property(t => t.IsManual).HasDefaultValue(false);

        builder.Property(t => t.Note).HasMaxLength(500);

        builder.HasIndex(t => new { t.UserId, t.ClockOut });
        builder.HasIndex(t => new { t.UserId, t.Status });
    }
}
