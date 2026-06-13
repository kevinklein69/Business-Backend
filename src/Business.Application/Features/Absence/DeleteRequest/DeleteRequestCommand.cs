using MediatR;

namespace Business.Application.Features.Absence.DeleteRequest;

public record DeleteRequestCommand(Guid Id) : IRequest;
