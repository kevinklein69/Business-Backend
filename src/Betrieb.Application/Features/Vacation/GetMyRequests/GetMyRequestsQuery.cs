using MediatR;

namespace Betrieb.Application.Features.Vacation.GetMyRequests;

public record GetMyRequestsQuery : IRequest<List<VacationRequestDto>>;
