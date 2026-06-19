using Business.Domain.Enums;
using MediatR;

namespace Business.Application.Features.CompanySettings.UpdateCompanySettings;

public record UpdateCompanySettingsCommand(
    GermanState State,
    string Street,
    string HouseNumber,
    string Zip,
    string City) : IRequest<CompanySettingsDto>;
