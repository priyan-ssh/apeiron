namespace Apeiron.Application.Contracts.Users;

public record UserRegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName);
