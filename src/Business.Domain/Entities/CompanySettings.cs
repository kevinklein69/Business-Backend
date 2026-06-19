using Business.Domain.Enums;

namespace Business.Domain.Entities;

/// Singleton settings row for the company (single-tenant application).
public class CompanySettings
{
    public Guid Id { get; set; }
    public GermanState State { get; set; }
    public string Street { get; set; } = string.Empty;
    public string HouseNumber { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}
