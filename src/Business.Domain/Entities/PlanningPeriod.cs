using Business.Domain.Enums;

namespace Business.Domain.Entities;

public class PlanningPeriod
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public PlanningPeriodStatus Status { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
