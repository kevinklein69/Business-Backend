using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Platform.DeleteCompany;

/// Hard-deletes a tenant and everything it owns. Runs cross-tenant (no logged-in company),
/// so every tenant-scoped table is queried with IgnoreQueryFilters() — otherwise the tenant
/// filter (CompanyId == null for an anonymous caller) would match nothing and delete nothing.
///
/// Order matters: deleting Orders and Users lets the database cascade their children
/// (order attachments/acceptances, order/user assignments, time entries, absence requests);
/// the loose tenant tables (no FK to Companies) are cleared explicitly; the Company row goes
/// last because Users -> Companies is ON DELETE RESTRICT.
///
/// ponytail: the table list is explicit — if a new tenant-scoped table is added, add it here too.
public class DeleteCompanyCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteCompanyCommand>
{
    public async Task Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var id = request.Id;

        var exists = await context.Companies.AnyAsync(c => c.Id == id, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException(nameof(Company), id);
        }

        await using var tx = await context.Database.BeginTransactionAsync(cancellationToken);

        // Cascades OrderAcceptances, OrderAttachments and order/user assignments; nulls TimeEntries.OrderId.
        await context.Orders.IgnoreQueryFilters()
            .Where(x => x.CompanyId == id).ExecuteDeleteAsync(cancellationToken);
        // Cascades AbsenceRequests, TimeEntries and user assignments.
        await context.Users.IgnoreQueryFilters()
            .Where(x => x.CompanyId == id).ExecuteDeleteAsync(cancellationToken);
        // Defensive: catch any rows a cascade above didn't reach (e.g. a nullable owner FK).
        await context.TimeEntries.IgnoreQueryFilters()
            .Where(x => x.CompanyId == id).ExecuteDeleteAsync(cancellationToken);
        await context.AbsenceRequests.IgnoreQueryFilters()
            .Where(x => x.CompanyId == id).ExecuteDeleteAsync(cancellationToken);
        // Loose tenant tables with no FK to Companies.
        await context.PlanningPeriods.IgnoreQueryFilters()
            .Where(x => x.CompanyId == id).ExecuteDeleteAsync(cancellationToken);
        await context.CompanySettings.IgnoreQueryFilters()
            .Where(x => x.CompanyId == id).ExecuteDeleteAsync(cancellationToken);

        await context.Companies
            .Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);

        await tx.CommitAsync(cancellationToken);
    }
}
