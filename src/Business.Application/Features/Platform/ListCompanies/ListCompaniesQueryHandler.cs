using Business.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Platform.ListCompanies;

public class ListCompaniesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<ListCompaniesQuery, IReadOnlyList<CompanyListItem>>
{
    public async Task<IReadOnlyList<CompanyListItem>> Handle(
        ListCompaniesQuery request, CancellationToken cancellationToken)
    {
        var companies = await context.Companies
            .OrderBy(c => c.Name)
            .Select(c => new { c.Id, c.Name, c.CreatedAt })
            .ToListAsync(cancellationToken);

        // Users is tenant-filtered; ignore the filter to count across all companies at once.
        var counts = await context.Users
            .IgnoreQueryFilters()
            .GroupBy(u => u.CompanyId)
            .Select(g => new { CompanyId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CompanyId, x => x.Count, cancellationToken);

        return companies
            .Select(c => new CompanyListItem(c.Id, c.Name, c.CreatedAt, counts.GetValueOrDefault(c.Id)))
            .ToList();
    }
}
