using MediatR;

namespace Business.Application.Features.TimeTracking.CreateManualEntry;

public record CreateManualEntryCommand(DateOnly Date, TimeOnly StartTime, TimeOnly EndTime, string Note)
    : IRequest<TimeEntryDto>;
