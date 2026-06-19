using Business.Application.Common.Interfaces;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.CompanySettings.GetCompanySettings;

public class GetCompanySettingsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetCompanySettingsQuery, CompanySettingsDto>
{
    public async Task<CompanySettingsDto> Handle(GetCompanySettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await context.CompanySettings.FirstOrDefaultAsync(cancellationToken);
        return new CompanySettingsDto(
            settings?.State ?? GermanState.Bayern,
            settings?.Street ?? string.Empty,
            settings?.HouseNumber ?? string.Empty,
            settings?.Zip ?? string.Empty,
            settings?.City ?? string.Empty);
    }
}
