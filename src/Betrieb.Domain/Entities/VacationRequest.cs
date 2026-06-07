using Betrieb.Domain.Enums;

namespace Betrieb.Domain.Entities;

public class VacationRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public VacationStatus Status { get; set; }
    public string? Comment { get; set; }
}
