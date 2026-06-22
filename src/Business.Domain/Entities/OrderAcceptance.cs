using Business.Domain.Common;

namespace Business.Domain.Entities;

public class OrderAcceptance : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid OrderId { get; set; }
    public string SignerName { get; set; } = string.Empty;
    public DateTime SignedAt { get; set; }
    public string SignatureImage { get; set; } = string.Empty;
}
