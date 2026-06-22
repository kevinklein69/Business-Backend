namespace Business.Domain.Common;

/// Marks an entity as belonging to a single tenant (company). The DbContext applies a
/// global query filter on <see cref="CompanyId"/> and auto-populates it on insert, so
/// handlers never have to filter or set the tenant manually.
public interface ITenantScoped
{
    Guid CompanyId { get; set; }
}
