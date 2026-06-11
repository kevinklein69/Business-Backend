using Business.Domain.Enums;
using MediatR;

namespace Business.Application.Features.Absence.CreateRequest;

public record CreateRequestCommand(AbsenceType Type, DateOnly StartDate, DateOnly EndDate, string? Comment)
    : IRequest<AbsenceRequestDto>;
