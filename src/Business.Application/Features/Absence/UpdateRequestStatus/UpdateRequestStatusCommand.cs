using Business.Domain.Enums;
using MediatR;

namespace Business.Application.Features.Absence.UpdateRequestStatus;

public record UpdateRequestStatusCommand(Guid Id, AbsenceStatus Status) : IRequest<AbsenceRequestDto>;
