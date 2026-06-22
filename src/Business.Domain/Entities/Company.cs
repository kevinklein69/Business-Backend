namespace Business.Domain.Entities;

/// Tenant root. One row per customer company; every tenant-scoped entity points back here
/// via CompanyId. Created when a company is onboarded (see RegisterCompany).
public class Company
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}
