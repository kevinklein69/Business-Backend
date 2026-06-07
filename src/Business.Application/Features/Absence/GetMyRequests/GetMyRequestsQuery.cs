using MediatR;

namespace Business.Application.Features.Absence.GetMyRequests;

public record GetMyRequestsQuery : IRequest<List<AbsenceRequestDto>>;
