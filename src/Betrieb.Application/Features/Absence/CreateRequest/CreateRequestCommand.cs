using MediatR;

namespace Betrieb.Application.Features.Absence.CreateRequest;

public record CreateRequestCommand(DateOnly StartDate, DateOnly EndDate, string? Comment)
    : IRequest<AbsenceRequestDto>;
