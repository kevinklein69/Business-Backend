using Betrieb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Betrieb.Infrastructure.Persistence.Configurations;

public class VacationRequestConfiguration : IEntityTypeConfiguration<VacationRequest>
{
    public void Configure(EntityTypeBuilder<VacationRequest> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Comment).HasMaxLength(500);
        builder.Property(v => v.Status).HasConversion<string>().HasMaxLength(20);
    }
}
