using Business.Domain.Enums;
using MediatR;

namespace Business.Application.Features.Absence.UpdateRequest;

public record UpdateRequestCommand(Guid Id, AbsenceType Type, DateOnly StartDate, DateOnly EndDate, string? Comment)
    : IRequest<AbsenceRequestDto>;
