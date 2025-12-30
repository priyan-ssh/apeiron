using Apeiron.Application.Interfaces.Auth;
using Apeiron.Domain.Entities;
using Apeiron.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Apeiron.Infrastructure.Seeders;

public class DataSeeder
{
    private readonly ApeironDbContext _context;
    private readonly UserManager<User> _userManager;

    public DataSeeder(ApeironDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task SeedAsync()
    {
        // Auto-Apply Migrations
        if (_context.Database.GetPendingMigrations().Any())
        {
            await _context.Database.MigrateAsync();
        }

        // Seed Users
        if (!await _userManager.Users.AnyAsync())
        {
            var admin = new User
            {
                UserName = "admin@apeiron.com",
                Email = "admin@apeiron.com",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userManager.CreateAsync(admin, "Admin123!");
        }

        // Seed Projects
        if (!await _context.Projects.AnyAsync())
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = "Project Zero (Seeded)",
                Description = "This is the Alpha project seeded by the system.",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
        }
    }
}
