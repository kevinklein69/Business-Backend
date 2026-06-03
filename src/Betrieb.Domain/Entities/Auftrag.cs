namespace Betrieb.Domain.Entities;

public class Auftrag
{
    public Guid Id { get; set; }
    public string Titel { get; set; } = string.Empty;
    public string Beschreibung { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ErstelltAm { get; set; }
}
