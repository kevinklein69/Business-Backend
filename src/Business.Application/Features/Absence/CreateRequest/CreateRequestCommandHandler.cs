using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Absence.CreateRequest;

public class CreateRequestCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<CreateRequestCommand, AbsenceRequestDto>
{
    public async Task<AbsenceRequestDto> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var absenceRequest = new AbsenceRequest
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = request.Type,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = AbsenceStatus.Open,
            Comment = request.Comment
        };

        context.AbsenceRequests.Add(absenceRequest);
        await context.SaveChangesAsync(cancellationToken);

        var saved = await context.AbsenceRequests
            .Include(a => a.User)
            .FirstAsync(a => a.Id == absenceRequest.Id, cancellationToken);

        return saved.ToDto(await context.GetCompanyStateAsync(cancellationToken));
    }
}
