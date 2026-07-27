using MediatR;

namespace Business.Application.Features.Platform.DeleteCompany;

/// Hard-deletes a company and everything it owns. Platform-admin only.
public record DeleteCompanyCommand(Guid Id) : IRequest;
