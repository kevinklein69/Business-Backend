using Business.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Common.Interfaces;

public static class CompanySettingsExtensions
{
    /// The Bundesland the company operates in, used to determine which public
    /// holidays apply when calculating absence business days. Falls back to
    /// Bayern if no settings row exists yet.
    public static async Task<GermanState> GetCompanyStateAsync(
        this IApplicationDbContext context, CancellationToken cancellationToken)
    {
        var settings = await context.CompanySettings.FirstOrDefaultAsync(cancellationToken);
        return settings?.State ?? GermanState.Bayern;
    }
}
