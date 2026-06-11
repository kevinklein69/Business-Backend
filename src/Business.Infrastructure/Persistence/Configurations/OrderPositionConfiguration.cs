using Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Business.Infrastructure.Persistence.Configurations;

public class OrderPositionConfiguration : IEntityTypeConfiguration<OrderPosition>
{
    public void Configure(EntityTypeBuilder<OrderPosition> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Description).IsRequired().HasMaxLength(500);
        builder.Property(p => p.Quantity).HasPrecision(10, 2);
        builder.Property(p => p.UnitPrice).HasPrecision(12, 2);
    }
}
