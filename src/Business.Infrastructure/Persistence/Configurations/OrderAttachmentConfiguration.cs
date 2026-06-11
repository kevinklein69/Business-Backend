using Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Business.Infrastructure.Persistence.Configurations;

public class OrderAttachmentConfiguration : IEntityTypeConfiguration<OrderAttachment>
{
    public void Configure(EntityTypeBuilder<OrderAttachment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName).IsRequired().HasMaxLength(255);
        builder.Property(a => a.ContentType).IsRequired().HasMaxLength(100);
        builder.Property(a => a.StoragePath).IsRequired().HasMaxLength(500);
    }
}
