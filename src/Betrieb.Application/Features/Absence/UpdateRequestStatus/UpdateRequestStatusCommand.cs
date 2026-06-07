using Betrieb.Domain.Enums;
using MediatR;

namespace Betrieb.Application.Features.Absence.UpdateRequestStatus;

public record UpdateRequestStatusCommand(Guid Id, AbsenceStatus Status) : IRequest<AbsenceRequestDto>;
