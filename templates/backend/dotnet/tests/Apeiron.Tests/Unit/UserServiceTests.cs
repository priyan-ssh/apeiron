using Apeiron.Application.Common.Models;
using Apeiron.Application.Contracts.Users;
using Apeiron.Application.Interfaces.Auth;
using Apeiron.Application.Services.Users;
using Apeiron.Domain.Entities;
using Apeiron.Application.Interfaces.Users;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace Apeiron.Tests.Unit;

public class UserServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<User> _userManager;
    private readonly IValidator<UserRegisterRequest> _validator;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly UserService _sut; // System Under Test

    public UserServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        var store = Substitute.For<IUserStore<User>>();
        _userManager = Substitute.For<UserManager<User>>(store, null, null, null, null, null, null, null, null);
        _validator = Substitute.For<IValidator<UserRegisterRequest>>();
        _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
        
        _sut = new UserService(_userRepository, _userManager, _validator, _jwtTokenGenerator);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new LoginRequest("test@test.com", "Password123!");
        var user = new User { Id = Guid.NewGuid(), Email = request.Email, FirstName = "Test", LastName = "User" };

        _userManager.FindByEmailAsync(request.Email).Returns(user);
        _userManager.CheckPasswordAsync(user, request.Password).Returns(true);
        _jwtTokenGenerator.GenerateToken(user).Returns("valid_token");

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().Be("valid_token");
        result.Value.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var request = new LoginRequest("unknown@test.com", "Password123!");
        _userManager.FindByEmailAsync(request.Email).Returns((User?)null);

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Auth.InvalidCredentials");
    }
}
