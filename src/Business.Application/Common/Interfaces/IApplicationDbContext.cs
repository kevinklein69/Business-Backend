using Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderPosition> OrderPositions { get; }
    DbSet<OrderAttachment> OrderAttachments { get; }
    DbSet<TimeEntry> TimeEntries { get; }
    DbSet<AbsenceRequest> AbsenceRequests { get; }
    DbSet<CompanySettings> CompanySettings { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
