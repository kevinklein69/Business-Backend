using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Orders.UploadAttachments;

public class UploadOrderAttachmentsCommandHandler(IApplicationDbContext context, IFileStorageService fileStorage)
    : IRequestHandler<UploadOrderAttachmentsCommand, List<OrderAttachmentDto>>
{
    public async Task<List<OrderAttachmentDto>> Handle(UploadOrderAttachmentsCommand request, CancellationToken cancellationToken)
    {
        var orderExists = await context.Orders
            .AnyAsync(o => o.Id == request.OrderId, cancellationToken);

        if (!orderExists)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        var savedPaths = new List<string>();
        var attachments = new List<OrderAttachment>();

        try
        {
            foreach (var file in request.Files)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var storagePath = await fileStorage.SaveAsync(
                    file.Content,
                    Path.Combine("orders", request.OrderId.ToString()),
                    extension,
                    cancellationToken);
                savedPaths.Add(storagePath);

                attachments.Add(new OrderAttachment
                {
                    Id = Guid.NewGuid(),
                    OrderId = request.OrderId,
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    SizeBytes = file.SizeBytes,
                    StoragePath = storagePath,
                    UploadedAt = DateTime.UtcNow
                });
            }

            context.OrderAttachments.AddRange(attachments);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            foreach (var path in savedPaths)
            {
                try
                {
                    await fileStorage.DeleteAsync(path);
                }
                catch
                {
                    // Aufräumen nach Fehlschlag ist best-effort; der ursprüngliche Fehler hat Vorrang.
                }
            }

            throw;
        }

        return attachments
            .Select(a => new OrderAttachmentDto(a.Id, a.FileName, a.ContentType, a.SizeBytes, a.UploadedAt))
            .ToList();
    }
}
