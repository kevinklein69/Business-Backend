using Business.Domain.Enums;

namespace Business.Domain.Entities;

public class TimeEntry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }
    public TimeEntryStatus Status { get; set; } = TimeEntryStatus.Approved;
    public bool IsManual { get; set; }
    public string? Note { get; set; }
}
