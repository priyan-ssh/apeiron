using Apeiron.Application.Interfaces.Users;
using Apeiron.Domain.Entities;
using Apeiron.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Apeiron.Infrastructure.Repositories.Users;

public class UserRepository : IUserRepository
{
    private readonly ApeironDbContext _context;

    public UserRepository(ApeironDbContext context) => _context = context;

    public async Task<User?> GetByIdAsync(Guid id) => await _context.Users.FindAsync(id);

    public async Task<User?> GetByEmailAsync(string email) 
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}
