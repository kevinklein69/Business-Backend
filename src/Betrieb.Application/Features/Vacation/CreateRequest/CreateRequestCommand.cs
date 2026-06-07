using MediatR;

namespace Betrieb.Application.Features.Vacation.CreateRequest;

public record CreateRequestCommand(DateOnly StartDate, DateOnly EndDate, string? Comment)
    : IRequest<VacationRequestDto>;
