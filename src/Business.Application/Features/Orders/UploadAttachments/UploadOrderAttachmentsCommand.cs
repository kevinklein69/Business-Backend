using MediatR;

namespace Business.Application.Features.Orders.UploadAttachments;

public record AttachmentUpload(string FileName, string ContentType, long SizeBytes, Stream Content);

public record UploadOrderAttachmentsCommand(Guid OrderId, List<AttachmentUpload> Files)
    : IRequest<List<OrderAttachmentDto>>;
