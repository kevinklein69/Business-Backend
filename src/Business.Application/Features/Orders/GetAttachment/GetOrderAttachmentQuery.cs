using MediatR;

namespace Business.Application.Features.Orders.GetAttachment;

public record AttachmentFileResult(Stream Content, string FileName, string ContentType);

public record GetOrderAttachmentQuery(Guid OrderId, Guid AttachmentId) : IRequest<AttachmentFileResult>;
