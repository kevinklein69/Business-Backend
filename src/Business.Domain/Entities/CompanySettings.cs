using Business.Domain.Enums;

namespace Business.Domain.Entities;

/// Singleton settings row for the company (single-tenant application).
public class CompanySettings
{
    public Guid Id { get; set; }
    public GermanState State { get; set; }
}
