using System.Reflection;
using Betrieb.Application.Common.Interfaces;
using Betrieb.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Betrieb.Infrastructure.Persistence;

public class BetriebDbContext(DbContextOptions<BetriebDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();
    public DbSet<AbsenceRequest> AbsenceRequests => Set<AbsenceRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
