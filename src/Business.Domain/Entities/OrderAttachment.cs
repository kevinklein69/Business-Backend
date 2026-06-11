namespace Business.Domain.Entities;

public class OrderAttachment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}
