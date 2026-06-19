using Business.Application.Common.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Application.Features.Auth.ChangePassword;

public class ChangePasswordCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    IPasswordHasher passwordHasher) : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var user = await context.Users.FirstAsync(u => u.Id == userId, cancellationToken);

        if (!passwordHasher.Verify(user, user.PasswordHash, request.CurrentPassword))
        {
            // 400 (not 401) on purpose: the frontend's axios interceptor force-logs-out on any
            // 401, which would kick the user to /login instead of just showing an error toast.
            throw new ValidationException(
                new[] { new ValidationFailure("CurrentPassword", "Aktuelles Passwort ist falsch.") });
        }

        user.PasswordHash = passwordHasher.Hash(user, request.NewPassword);
        await context.SaveChangesAsync(cancellationToken);
    }
}
