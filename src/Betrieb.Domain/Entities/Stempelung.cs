namespace Betrieb.Domain.Entities;

public class Stempelung
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime EingestempeltAm { get; set; }
    public DateTime? AusgestempeltAm { get; set; }
}
