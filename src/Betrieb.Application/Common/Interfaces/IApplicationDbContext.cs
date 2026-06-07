using Betrieb.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Betrieb.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Order> Orders { get; }
    DbSet<TimeEntry> TimeEntries { get; }
    DbSet<VacationRequest> VacationRequests { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
