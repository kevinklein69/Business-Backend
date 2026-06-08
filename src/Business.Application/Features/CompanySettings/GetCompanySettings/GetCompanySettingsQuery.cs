using MediatR;

namespace Business.Application.Features.CompanySettings.GetCompanySettings;

public record GetCompanySettingsQuery : IRequest<CompanySettingsDto>;
