using Apeiron.Domain.Entities;

namespace Apeiron.Application.Interfaces;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task RegisterUserAsync(User user, string password);
}
