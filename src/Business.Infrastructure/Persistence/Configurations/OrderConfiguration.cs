using Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Business.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Title).IsRequired().HasMaxLength(200);
        builder.Property(o => o.Description).HasMaxLength(2000);
        builder.Property(o => o.Customer).HasMaxLength(200);
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(30);

        builder.Property(o => o.Revenue).HasPrecision(12, 2);
        builder.Property(o => o.EstimatedHours).HasPrecision(7, 2);
        builder.Property(o => o.ActualHours).HasPrecision(7, 2);
        builder.Property(o => o.DeviationReason).HasMaxLength(500);

        builder.HasMany(o => o.Assignees)
            .WithMany(u => u.AssignedOrders)
            .UsingEntity(j => j.ToTable("OrderAssignments"));

        builder.HasMany(o => o.Positions)
            .WithOne()
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Attachments)
            .WithOne()
            .HasForeignKey(a => a.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Acceptance)
            .WithOne()
            .HasForeignKey<OrderAcceptance>(a => a.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
