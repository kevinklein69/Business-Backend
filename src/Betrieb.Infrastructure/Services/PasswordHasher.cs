using Betrieb.Application.Common.Interfaces;
using Betrieb.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Betrieb.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _identityHasher = new();

    public string Hash(User user, string password) =>
        _identityHasher.HashPassword(user, password);

    public bool Verify(User user, string hash, string password) =>
        _identityHasher.VerifyHashedPassword(user, hash, password) != PasswordVerificationResult.Failed;
}
