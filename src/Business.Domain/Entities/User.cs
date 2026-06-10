using Business.Domain.Enums;

namespace Business.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
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

    public ICollection<Order> AssignedOrders { get; set; } = new List<Order>();
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public ICollection<AbsenceRequest> AbsenceRequests { get; set; } = new List<AbsenceRequest>();
}
