using Betrieb.Domain.Entities;

namespace Betrieb.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
