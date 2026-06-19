using Business.Domain.Enums;

namespace Business.Application.Features.CompanySettings;

public record CompanySettingsDto(GermanState State, string Street, string HouseNumber, string Zip, string City);
