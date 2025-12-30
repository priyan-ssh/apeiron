namespace Apeiron.Application.Contracts.Projects;

public record ProjectResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt);
