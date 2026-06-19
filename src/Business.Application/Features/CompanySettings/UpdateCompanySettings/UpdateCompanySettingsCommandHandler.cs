using Business.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.CompanySettings.UpdateCompanySettings;

public class UpdateCompanySettingsCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateCompanySettingsCommand, CompanySettingsDto>
{
    public async Task<CompanySettingsDto> Handle(UpdateCompanySettingsCommand request, CancellationToken cancellationToken)
    {
        var settings = await context.CompanySettings.FirstOrDefaultAsync(cancellationToken);
        if (settings is null)
        {
            settings = new Domain.Entities.CompanySettings { Id = Guid.NewGuid() };
            context.CompanySettings.Add(settings);
        }

        settings.State = request.State;
        settings.Street = request.Street;
        settings.HouseNumber = request.HouseNumber;
        settings.Zip = request.Zip;
        settings.City = request.City;
        await context.SaveChangesAsync(cancellationToken);

        return new CompanySettingsDto(settings.State, settings.Street, settings.HouseNumber, settings.Zip, settings.City);
    }
}
