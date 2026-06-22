using Business.Application.Features.Auth.Login;
using MediatR;

namespace Business.Application.Features.Auth.RegisterCompany;

/// Onboards a new tenant: creates the Company plus its first Admin user, and returns a
/// ready-to-use auth token for that admin.
public record RegisterCompanyCommand(
    string CompanyName,
    string AdminFirstName,
    string AdminLastName,
    string AdminEmail,
    string AdminPassword) : IRequest<LoginResult>;
