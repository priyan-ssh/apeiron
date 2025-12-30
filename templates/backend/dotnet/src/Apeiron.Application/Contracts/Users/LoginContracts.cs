namespace Apeiron.Application.Contracts.Users;

public record LoginRequest(string Email, string Password);
public record LoginResponse(Guid Id, string Email, string FirstName, string LastName, string Token);
