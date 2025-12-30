using Apeiron.Domain.Entities;

namespace Apeiron.Application.Interfaces.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
}
