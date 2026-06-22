using Business.Domain.Common;
using Business.Domain.Enums;

namespace Business.Domain.Entities;

public class AbsenceRequest : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public AbsenceType Type { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public AbsenceStatus Status { get; set; }
    public string? Comment { get; set; }
}
