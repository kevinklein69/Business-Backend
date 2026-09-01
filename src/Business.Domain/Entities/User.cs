using Business.Domain.Common;
using Business.Domain.Enums;

namespace Business.Domain.Entities;

public class User : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; }
    public string? Department { get; set; }

    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public string? Zip { get; set; }
    public string? City { get; set; }
    public string? Phone { get; set; }
    public DateOnly? EntryDate { get; set; }
    public int? ProbationMonths { get; set; }
    public DateOnly? ProbationEndDate { get; set; }
    public int? VacationDaysEntitlement { get; set; }

    /// Zeitkonto balance (in minutes, may be negative) carried over from a previous
    /// system when the employee was onboarded into this software. Added on top of the
    /// balance accrued since their first actual time entry — EntryDate itself is never
    /// used to back-calculate a deficit, since it may predate the software's rollout.
    public int? InitialBalanceMinutes { get; set; }

    /// Vacation days already taken this calendar year in a previous system, before the
    /// employee was onboarded into this software (so they're not reflected in any
    /// AbsenceRequest here). Paired with InitialVacationYear so it only offsets
    /// RemainingVacationDays for the year it was recorded, and stops applying once a
    /// new year's entitlement starts.
    public decimal? InitialVacationDaysTaken { get; set; }
    public int? InitialVacationYear { get; set; }

    public ICollection<Order> AssignedOrders { get; set; } = new List<Order>();
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public ICollection<AbsenceRequest> AbsenceRequests { get; set; } = new List<AbsenceRequest>();
}
