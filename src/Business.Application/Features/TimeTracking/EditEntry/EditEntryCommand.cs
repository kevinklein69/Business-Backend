using MediatR;

namespace Business.Application.Features.TimeTracking.EditEntry;

public record EditEntryCommand(Guid Id, DateOnly Date, TimeOnly StartTime, TimeOnly EndTime, string Note)
    : IRequest<TimeEntryDto>;
