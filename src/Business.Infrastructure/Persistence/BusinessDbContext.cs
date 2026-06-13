using System.Reflection;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Infrastructure.Persistence;

public class BusinessDbContext(DbContextOptions<BusinessDbContext> options)
    : DbContext(options), IApplicationDbContext
{
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
        base.OnModelCreating(modelBuilder);
    }
}
