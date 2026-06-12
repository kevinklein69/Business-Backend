using Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Business.Infrastructure.Persistence.Configurations;

public class OrderAcceptanceConfiguration : IEntityTypeConfiguration<OrderAcceptance>
{
    public void Configure(EntityTypeBuilder<OrderAcceptance> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasIndex(a => a.OrderId).IsUnique();

        builder.Property(a => a.SignerName).IsRequired().HasMaxLength(200);
        builder.Property(a => a.SignatureImage).IsRequired();
    }
}
