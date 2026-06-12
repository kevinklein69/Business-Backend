using Business.Application.Common.Exceptions;
using Business.Application.Features.Orders.SignAcceptance;
using Business.Domain.Entities;
using Business.Domain.Enums;
using Business.UnitTests.TimeTracking;

namespace Business.UnitTests.Orders;

public class SignOrderAcceptanceCommandHandlerTests
{
    private const string SignatureImage = "data:image/png;base64,AQIDBA==";

    [Fact]
    public async Task Handle_OrderReadyForAcceptance_SignsAndGeneratesPdfAndCompletesOrder()
    {
        var context = TestDbContextFactory.Create();
        var orderId = Guid.NewGuid();
        context.Orders.Add(new Order
        {
            Id = orderId,
            Title = "Dachsanierung",
            Status = OrderStatus.ReadyForAcceptance,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var fileStorage = new FakeFileStorageService();
        var pdfGenerator = new FakeAcceptanceProtocolPdfGenerator();
        var handler = new SignOrderAcceptanceCommandHandler(context, fileStorage, pdfGenerator);

        var command = new SignOrderAcceptanceCommand(orderId, "Max Mustermann", SignatureImage);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(OrderStatus.Done, result.Status);
        Assert.Single(result.Attachments);
        Assert.Equal($"Abnahmeprotokoll_{orderId}.pdf", result.Attachments[0].FileName);
        Assert.Equal("application/pdf", result.Attachments[0].ContentType);
        Assert.Single(fileStorage.SavedPaths);

        var acceptance = context.OrderAcceptances.Single(a => a.OrderId == orderId);
        Assert.Equal("Max Mustermann", acceptance.SignerName);
        Assert.Equal(SignatureImage, acceptance.SignatureImage);

        Assert.Equal(orderId, pdfGenerator.LastOrder?.Id);
        Assert.Equal("Max Mustermann", pdfGenerator.LastAcceptance?.SignerName);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ThrowsNotFoundException()
    {
        var context = TestDbContextFactory.Create();
        var handler = new SignOrderAcceptanceCommandHandler(context, new FakeFileStorageService(), new FakeAcceptanceProtocolPdfGenerator());

        var command = new SignOrderAcceptanceCommand(Guid.NewGuid(), "Max Mustermann", SignatureImage);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_OrderNotReadyForAcceptance_ThrowsConflictException()
    {
        var context = TestDbContextFactory.Create();
        var orderId = Guid.NewGuid();
        context.Orders.Add(new Order
        {
            Id = orderId,
            Title = "Dachsanierung",
            Status = OrderStatus.InProgress,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var handler = new SignOrderAcceptanceCommandHandler(context, new FakeFileStorageService(), new FakeAcceptanceProtocolPdfGenerator());

        var command = new SignOrderAcceptanceCommand(orderId, "Max Mustermann", SignatureImage);

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(command, CancellationToken.None));
    }
}
