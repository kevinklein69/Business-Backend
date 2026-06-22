using System.Reflection;
using Business.Application.Common.Interfaces;
using Business.Domain.Common;
using Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Infrastructure.Persistence;

public class BusinessDbContext(
    DbContextOptions<BusinessDbContext> options,
    ICurrentUserService currentUser)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<PlanningPeriod> PlanningPeriods => Set<PlanningPeriod>();
    public DbSet<OrderAttachment> OrderAttachments => Set<OrderAttachment>();
    public DbSet<OrderAcceptance> OrderAcceptances => Set<OrderAcceptance>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();
    public DbSet<AbsenceRequest> AbsenceRequests => Set<AbsenceRequest>();
    public DbSet<CompanySettings> CompanySettings => Set<CompanySettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Tenant isolation: every query against a tenant-scoped entity is automatically
        // filtered to the current user's company. Use IgnoreQueryFilters() for the rare
        // cross-tenant operations (login by email, company registration, the dev seeder).
        // currentUser.CompanyId is evaluated per request, so the cached model stays valid.
        modelBuilder.Entity<User>().HasQueryFilter(e => e.CompanyId == currentUser.CompanyId);
        modelBuilder.Entity<Order>().HasQueryFilter(e => e.CompanyId == currentUser.CompanyId);
        modelBuilder.Entity<PlanningPeriod>().HasQueryFilter(e => e.CompanyId == currentUser.CompanyId);
        modelBuilder.Entity<TimeEntry>().HasQueryFilter(e => e.CompanyId == currentUser.CompanyId);
        modelBuilder.Entity<AbsenceRequest>().HasQueryFilter(e => e.CompanyId == currentUser.CompanyId);
        modelBuilder.Entity<OrderAttachment>().HasQueryFilter(e => e.CompanyId == currentUser.CompanyId);
        modelBuilder.Entity<OrderAcceptance>().HasQueryFilter(e => e.CompanyId == currentUser.CompanyId);
        modelBuilder.Entity<CompanySettings>().HasQueryFilter(e => e.CompanyId == currentUser.CompanyId);

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTenantId();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyTenantId();
        return base.SaveChanges();
    }

    /// Stamp newly added tenant-scoped entities with the current company so handlers never
    /// have to set CompanyId by hand. Skipped when there is no authenticated tenant
    /// (e.g. registration / seeder) — those set CompanyId explicitly.
    private void ApplyTenantId()
    {
        if (currentUser.CompanyId is not { } companyId)
        {
            return;
        }

        foreach (var entry in ChangeTracker.Entries<ITenantScoped>())
        {
            if (entry.State == EntityState.Added && entry.Entity.CompanyId == Guid.Empty)
            {
                entry.Entity.CompanyId = companyId;
            }
        }
    }
}
