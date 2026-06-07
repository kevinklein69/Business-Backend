using MediatR;

namespace Business.Application.Features.Absence.CreateRequest;

public record CreateRequestCommand(DateOnly StartDate, DateOnly EndDate, string? Comment)
    : IRequest<AbsenceRequestDto>;
