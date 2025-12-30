namespace Apeiron.Application.Contracts.Users;

public record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAt);
