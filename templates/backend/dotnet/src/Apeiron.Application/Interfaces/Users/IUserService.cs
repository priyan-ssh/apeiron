using Apeiron.Application.Common.Models;
using Apeiron.Application.Contracts.Users;

namespace Apeiron.Application.Interfaces.Users;

public interface IUserService
{
    Task<Result<UserResponse>> GetByIdAsync(Guid id);
    Task<Result<UserResponse>> GetByEmailAsync(string email);
    Task<Result<UserResponse>> RegisterUserAsync(UserRegisterRequest request);
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
}
