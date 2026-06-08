using Business.Domain.Enums;
using MediatR;

namespace Business.Application.Features.CompanySettings.UpdateCompanySettings;

public record UpdateCompanySettingsCommand(GermanState State) : IRequest<CompanySettingsDto>;
