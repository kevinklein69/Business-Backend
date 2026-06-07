using MediatR;

namespace Betrieb.Application.Features.Absence.GetTeamRequests;

/// For managers/admins: lists absence requests across all employees.
public record GetTeamRequestsQuery : IRequest<List<AbsenceRequestDto>>;
