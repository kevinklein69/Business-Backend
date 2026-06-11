namespace Business.Domain.Entities;

public class OrderPosition
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int SortOrder { get; set; }
}
