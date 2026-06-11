using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using Business.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Business.Application.Features.PlanningPeriods.CreatePlanningPeriod;

public class CreatePlanningPeriodCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreatePlanningPeriodCommand, PlanningPeriodDto>
{
    public async Task<PlanningPeriodDto> Handle(CreatePlanningPeriodCommand request, CancellationToken cancellationToken)
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

        var period = new PlanningPeriod
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = PlanningPeriodStatus.Planned
        };

        context.PlanningPeriods.Add(period);
        await context.SaveChangesAsync(cancellationToken);

        return new PlanningPeriodDto(period.Id, period.Name, period.StartDate, period.EndDate, period.Status, 0);
    }
}
