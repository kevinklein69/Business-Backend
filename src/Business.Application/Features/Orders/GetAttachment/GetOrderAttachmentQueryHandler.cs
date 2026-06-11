using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Orders.GetAttachment;

public class GetOrderAttachmentQueryHandler(IApplicationDbContext context, IFileStorageService fileStorage)
    : IRequestHandler<GetOrderAttachmentQuery, AttachmentFileResult>
{
    public async Task<AttachmentFileResult> Handle(GetOrderAttachmentQuery request, CancellationToken cancellationToken)
    {
        var attachment = await context.OrderAttachments
            .FirstOrDefaultAsync(a => a.Id == request.AttachmentId && a.OrderId == request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(OrderAttachment), request.AttachmentId);

        var stream = fileStorage.OpenRead(attachment.StoragePath)
            ?? throw new NotFoundException(nameof(OrderAttachment), request.AttachmentId);

        return new AttachmentFileResult(stream, attachment.FileName, attachment.ContentType);
    }
}
