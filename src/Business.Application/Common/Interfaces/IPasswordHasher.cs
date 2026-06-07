using Business.Domain.Entities;

namespace Business.Application.Common.Interfaces;

public interface IPasswordHasher
{
    string Hash(User user, string password);

    bool Verify(User user, string hash, string password);
}
