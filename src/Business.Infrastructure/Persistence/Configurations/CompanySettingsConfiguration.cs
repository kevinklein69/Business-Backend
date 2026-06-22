using Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Business.Infrastructure.Persistence.Configurations;

public class CompanySettingsConfiguration : IEntityTypeConfiguration<CompanySettings>
{
    public void Configure(EntityTypeBuilder<CompanySettings> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.CompanyId).IsRequired();
        // One settings row per company.
        builder.HasIndex(s => s.CompanyId).IsUnique();

        builder.Property(s => s.State).HasConversion<string>().HasMaxLength(30);
    }
}
