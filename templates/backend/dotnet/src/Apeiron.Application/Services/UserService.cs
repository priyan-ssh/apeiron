using Apeiron.Application.Interfaces;
using Apeiron.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Apeiron.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<User> _userManager;

    public UserService(IUserRepository userRepository, UserManager<User> userManager)
    {
        _userRepository = userRepository;
        _userManager = userManager;
    }

    public async Task<User?> GetByIdAsync(Guid id) => await _userRepository.GetByIdAsync(id);

    public async Task<User?> GetByEmailAsync(string email) => await _userRepository.GetByEmailAsync(email);

    public async Task RegisterUserAsync(User user, string password)
    {
        // Use Identity's UserManager for secure password hashing and creation
        var result = await _userManager.CreateAsync(user, password);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"User registration failed: {errors}");
        }
    }
}
