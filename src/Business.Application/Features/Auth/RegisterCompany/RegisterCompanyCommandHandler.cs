using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Application.Features.Auth.Login;
using Business.Domain.Entities;
using Business.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Auth.RegisterCompany;

public class RegisterCompanyCommandHandler(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator tokenGenerator) : IRequestHandler<RegisterCompanyCommand, LoginResult>
{
    public async Task<LoginResult> Handle(RegisterCompanyCommand request, CancellationToken cancellationToken)
    {
        // Email is globally unique; check across all tenants (no tenant context exists yet anyway).
        var emailTaken = await context.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email == request.AdminEmail, cancellationToken);
        if (emailTaken)
        {
            throw new ConflictException("Diese E-Mail ist bereits vergeben.");
        }

        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = request.CompanyName,
            CreatedAt = DateTime.UtcNow,
        };
        context.Companies.Add(company);

        var admin = new User
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            FirstName = request.AdminFirstName,
            LastName = request.AdminLastName,
            Email = request.AdminEmail,
            Role = Role.Admin,
        };
        admin.PasswordHash = passwordHasher.Hash(admin, request.AdminPassword);
        context.Users.Add(admin);

        context.CompanySettings.Add(new Domain.Entities.CompanySettings
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            State = GermanState.Bayern,
        });

        await context.SaveChangesAsync(cancellationToken);

        return new LoginResult(tokenGenerator.GenerateToken(admin));
    }
}
