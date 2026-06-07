using Betrieb.Domain.Enums;
using MediatR;

namespace Betrieb.Application.Features.Absence.RecordAbsence;

/// Lets a manager/admin record an absence (e.g. sick leave, child sick) on behalf of an
/// employee. The resulting request is created as already approved, since the manager is
/// documenting a fact rather than asking for permission.
public record RecordAbsenceCommand(Guid UserId, AbsenceType Type, DateOnly StartDate, DateOnly EndDate, string? Comment)
    : IRequest<AbsenceRequestDto>;
