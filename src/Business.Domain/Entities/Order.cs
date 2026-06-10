using Business.Domain.Enums;

namespace Business.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Customer { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public decimal? Revenue { get; set; }
    public DateOnly? InvoiceDate { get; set; }
    public decimal? EstimatedHours { get; set; }
    public DateOnly? PlannedStartDate { get; set; }
    public DateOnly? PlannedEndDate { get; set; }
    public decimal? ActualHours { get; set; }
    public string? DeviationReason { get; set; }

    public ICollection<User> Assignees { get; set; } = new List<User>();
}
