using Betrieb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Betrieb.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Title).IsRequired().HasMaxLength(200);
        builder.Property(o => o.Description).HasMaxLength(2000);
        builder.Property(o => o.Customer).HasMaxLength(200);
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(30);

        builder.HasMany(o => o.Assignees)
            .WithMany(u => u.AssignedOrders)
            .UsingEntity(j => j.ToTable("OrderAssignments"));
    }
}
