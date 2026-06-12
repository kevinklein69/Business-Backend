using MediatR;

namespace Business.Application.Features.Orders.SignAcceptance;

public record SignOrderAcceptanceCommand(Guid OrderId, string SignerName, string SignatureImageBase64) : IRequest<OrderDto>;
