using Betrieb.Domain.Enums;
using MediatR;

namespace Betrieb.Application.Features.Vacation.UpdateRequestStatus;

public record UpdateRequestStatusCommand(Guid Id, VacationStatus Status) : IRequest<VacationRequestDto>;
