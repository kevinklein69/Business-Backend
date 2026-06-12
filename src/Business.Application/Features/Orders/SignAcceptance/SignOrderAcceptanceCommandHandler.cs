using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Orders.SignAcceptance;

public class SignOrderAcceptanceCommandHandler(
    IApplicationDbContext context,
    IFileStorageService fileStorage,
    IAcceptanceProtocolPdfGenerator pdfGenerator)
    : IRequestHandler<SignOrderAcceptanceCommand, OrderDto>
{
    public async Task<OrderDto> Handle(SignOrderAcceptanceCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .Include(o => o.Assignees)
            .Include(o => o.Positions)
            .Include(o => o.Attachments)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        if (order.Status != OrderStatus.ReadyForAcceptance)
        {
            throw new ConflictException("Der Auftrag muss sich im Status 'Ready für Abnahme' befinden.");
        }

        var acceptance = await context.OrderAcceptances
            .FirstOrDefaultAsync(a => a.OrderId == order.Id, cancellationToken);

        if (acceptance is null)
        {
            acceptance = new OrderAcceptance
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id
            };
            context.OrderAcceptances.Add(acceptance);
        }

        acceptance.SignerName = request.SignerName;
        acceptance.SignedAt = DateTime.UtcNow;
        acceptance.SignatureImage = request.SignatureImageBase64;

        var pdfBytes = pdfGenerator.Generate(order, acceptance);

        var storagePath = await fileStorage.SaveAsync(
            new MemoryStream(pdfBytes),
            Path.Combine("orders", order.Id.ToString()),
            ".pdf",
            cancellationToken);

        try
        {
            var attachment = new OrderAttachment
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                FileName = $"Abnahmeprotokoll_{order.Id}.pdf",
                ContentType = "application/pdf",
                SizeBytes = pdfBytes.LongLength,
                StoragePath = storagePath,
                UploadedAt = DateTime.UtcNow
            };

            context.OrderAttachments.Add(attachment);

            order.Status = OrderStatus.Done;

            await context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            try
            {
                await fileStorage.DeleteAsync(storagePath);
            }
            catch
            {
                // Aufräumen nach Fehlschlag ist best-effort; der ursprüngliche Fehler hat Vorrang.
            }

            throw;
        }

        return OrderMapping.ToDto(order);
    }
}
