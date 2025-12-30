using Apeiron.Domain.Entities;

namespace Apeiron.Application.Interfaces.Auth;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
