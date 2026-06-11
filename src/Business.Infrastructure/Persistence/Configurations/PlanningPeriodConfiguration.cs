using Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Business.Infrastructure.Persistence.Configurations;

public class PlanningPeriodConfiguration : IEntityTypeConfiguration<PlanningPeriod>
{
    public void Configure(EntityTypeBuilder<PlanningPeriod> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(120);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasMany(p => p.Orders)
            .WithOne(o => o.PlanningPeriod)
            .HasForeignKey(o => o.PlanningPeriodId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
