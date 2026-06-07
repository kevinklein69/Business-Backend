using Betrieb.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Betrieb.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Order> Orders { get; }
    DbSet<TimeEntry> TimeEntries { get; }
    DbSet<AbsenceRequest> AbsenceRequests { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
