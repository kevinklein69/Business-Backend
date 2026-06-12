using Business.Domain.Enums;
using MediatR;

namespace Business.Application.Features.TimeTracking.GetPendingEntries;

/// For managers/admins: lists time entries awaiting approval across all employees.
public record GetPendingEntriesQuery : IRequest<List<PendingTimeEntryDto>>;

public record PendingTimeEntryDto(
    Guid Id,
    Guid UserId,
    string UserName,
    DateOnly Date,
    DateTime ClockIn,
    DateTime? ClockOut,
    bool IsManual,
    TimeEntryStatus Status,
    string? Note);
