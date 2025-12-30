using Apeiron.Application.Common.Models;
using Apeiron.Application.Interfaces.Auth;
using FluentValidation;
using Apeiron.Application.Contracts.Users;
using Apeiron.Application.Interfaces.Users;
using Apeiron.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Apeiron.Application.Services.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<User> _userManager;
    private readonly IValidator<UserRegisterRequest> _validator;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public UserService(IUserRepository userRepository, UserManager<User> userManager, IValidator<UserRegisterRequest> validator, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _validator = validator;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<UserResponse>> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
        {
            return Result<UserResponse>.Failure(new Error("User.NotFound", $"User with ID {id} not found."));
        }

        var response = new UserResponse(user.Id, user.Email!, user.FirstName, user.LastName, user.CreatedAt);
        return Result<UserResponse>.Success(response);
    }

    public async Task<Result<UserResponse>> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null)
        {
            return Result<UserResponse>.Failure(new Error("User.NotFound", $"User with email {email} not found."));
        }

        var response = new UserResponse(user.Id, user.Email!, user.FirstName, user.LastName, user.CreatedAt);
        return Result<UserResponse>.Success(response);
    }

    public async Task<Result<UserResponse>> RegisterUserAsync(UserRegisterRequest request)
    {
        _validator.ValidateAndThrow(request);

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
        {
            var firstError = result.Errors.First();
            return Result<UserResponse>.Failure(new Error(firstError.Code, firstError.Description));
        }

        var response = new UserResponse(user.Id, user.Email!, user.FirstName, user.LastName, user.CreatedAt);
        return Result<UserResponse>.Success(response);
    }
    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user is null)
        {
            return Result<LoginResponse>.Failure(new Error("Auth.InvalidCredentials", "Invalid email or password."));
        }

        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        
        if (!result)
        {
            return Result<LoginResponse>.Failure(new Error("Auth.InvalidCredentials", "Invalid email or password."));
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        var response = new LoginResponse(user.Id, user.Email!, user.FirstName, user.LastName, token);
        
        return Result<LoginResponse>.Success(response);
    }
}
