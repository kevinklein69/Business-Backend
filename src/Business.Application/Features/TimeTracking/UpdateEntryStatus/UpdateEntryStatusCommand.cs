using Business.Domain.Enums;
using MediatR;

namespace Business.Application.Features.TimeTracking.UpdateEntryStatus;

public record UpdateEntryStatusCommand(Guid Id, TimeEntryStatus Status) : IRequest<TimeEntryDto>;
