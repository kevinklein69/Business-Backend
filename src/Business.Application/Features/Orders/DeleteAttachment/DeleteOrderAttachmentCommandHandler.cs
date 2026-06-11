using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Orders.DeleteAttachment;

public class DeleteOrderAttachmentCommandHandler(IApplicationDbContext context, IFileStorageService fileStorage)
    : IRequestHandler<DeleteOrderAttachmentCommand>
{
    public async Task Handle(DeleteOrderAttachmentCommand request, CancellationToken cancellationToken)
    {
        var attachment = await context.OrderAttachments
            .FirstOrDefaultAsync(a => a.Id == request.AttachmentId && a.OrderId == request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(OrderAttachment), request.AttachmentId);

        context.OrderAttachments.Remove(attachment);
        await context.SaveChangesAsync(cancellationToken);

        try
        {
            await fileStorage.DeleteAsync(attachment.StoragePath);
        }
        catch
        {
            // Datei-Löschen ist best-effort; die DB-Row ist bereits entfernt.
        }
    }
}
