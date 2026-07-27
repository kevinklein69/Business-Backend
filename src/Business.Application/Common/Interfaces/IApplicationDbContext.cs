using Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Business.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Company> Companies { get; }
    DbSet<User> Users { get; }
    DbSet<Order> Orders { get; }
    DbSet<PlanningPeriod> PlanningPeriods { get; }
    DbSet<OrderAttachment> OrderAttachments { get; }
    DbSet<OrderAcceptance> OrderAcceptances { get; }
    DbSet<TimeEntry> TimeEntries { get; }
    DbSet<AbsenceRequest> AbsenceRequests { get; }
    DbSet<CompanySettings> CompanySettings { get; }

    // Needed for cross-tenant bulk operations that must be atomic (e.g. deleting a whole company).
    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
