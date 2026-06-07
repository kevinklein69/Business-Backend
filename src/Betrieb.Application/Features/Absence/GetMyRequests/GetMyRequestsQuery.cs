using MediatR;

namespace Betrieb.Application.Features.Absence.GetMyRequests;

public record GetMyRequestsQuery : IRequest<List<AbsenceRequestDto>>;
