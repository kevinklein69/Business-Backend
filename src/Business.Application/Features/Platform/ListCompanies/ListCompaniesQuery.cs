using MediatR;

namespace Business.Application.Features.Platform.ListCompanies;

public record CompanyListItem(Guid Id, string Name, DateTime CreatedAt, int UserCount);

/// Cross-tenant: lists every company. Used only by the platform-admin surface.
public record ListCompaniesQuery : IRequest<IReadOnlyList<CompanyListItem>>;
