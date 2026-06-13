using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Orders.DeleteOrder;

public class DeleteOrderCommandHandler(IApplicationDbContext context, IFileStorageService fileStorage)
    : IRequestHandler<DeleteOrderCommand>
{
    public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .Include(o => o.Attachments)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.Id);

        var attachmentPaths = order.Attachments.Select(a => a.StoragePath).ToList();

        context.Orders.Remove(order);
        await context.SaveChangesAsync(cancellationToken);

        foreach (var path in attachmentPaths)
        {
            try
            {
                await fileStorage.DeleteAsync(path);
            }
            catch
            {
                // Datei-Löschen ist best-effort; die DB-Row ist bereits entfernt.
            }
        }
    }
}
