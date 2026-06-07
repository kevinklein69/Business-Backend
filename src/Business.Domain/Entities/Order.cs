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

    public ICollection<User> Assignees { get; set; } = new List<User>();
}
