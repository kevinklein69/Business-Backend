using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using Business.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.PlanningPeriods.UpdatePlanningPeriod;

public class UpdatePlanningPeriodCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdatePlanningPeriodCommand, PlanningPeriodDto>
{
    public async Task<PlanningPeriodDto> Handle(UpdatePlanningPeriodCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException(
            [
                new ValidationFailure("Name", "Der Name des Planungszeitraums darf nicht leer sein.")
            ]);
        }

        if (request.StartDate is null || request.EndDate is null)
        {
            throw new ValidationException(
            [
                new ValidationFailure("StartDate", "Start- und Enddatum sind Pflichtfelder.")
            ]);
        }

        if (request.EndDate < request.StartDate)
        {
            throw new ValidationException(
            [
                new ValidationFailure("EndDate", "Das Enddatum darf nicht vor dem Startdatum liegen.")
            ]);
        }

        var period = await context.PlanningPeriods
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(PlanningPeriod), request.Id);

        // Invariante: es darf immer nur genau einen aktiven Zeitraum geben.
        if (request.Status == PlanningPeriodStatus.Active && period.Status != PlanningPeriodStatus.Active)
        {
            var currentlyActive = await context.PlanningPeriods
                .Where(p => p.Status == PlanningPeriodStatus.Active && p.Id != period.Id)
                .ToListAsync(cancellationToken);

            foreach (var other in currentlyActive)
            {
                other.Status = PlanningPeriodStatus.Closed;
            }
        }

        period.Name = request.Name.Trim();
        period.StartDate = request.StartDate;
        period.EndDate = request.EndDate;
        period.Status = request.Status;

        await context.SaveChangesAsync(cancellationToken);

        var orderCount = await context.Orders.CountAsync(o => o.PlanningPeriodId == period.Id, cancellationToken);

        return new PlanningPeriodDto(period.Id, period.Name, period.StartDate, period.EndDate, period.Status, orderCount);
    }
}
