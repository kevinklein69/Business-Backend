using Business.Domain.Common;
using Business.Domain.Enums;

namespace Business.Domain.Entities;

/// Per-company settings row (one per tenant). Scoped via CompanyId.
public class CompanySettings : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public GermanState State { get; set; }
    public string Street { get; set; } = string.Empty;
    public string HouseNumber { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}
