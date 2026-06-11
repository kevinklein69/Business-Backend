using MediatR;

namespace Business.Application.Features.Orders.DeleteAttachment;

public record DeleteOrderAttachmentCommand(Guid OrderId, Guid AttachmentId) : IRequest;
