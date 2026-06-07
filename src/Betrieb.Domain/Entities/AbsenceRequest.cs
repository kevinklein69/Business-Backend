using Betrieb.Domain.Enums;

namespace Betrieb.Domain.Entities;

public class AbsenceRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public AbsenceType Type { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public AbsenceStatus Status { get; set; }
    public string? Comment { get; set; }
}
