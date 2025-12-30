namespace Apeiron.Application.Contracts.Projects;

public record ProjectCreateRequest(
    string Name,
    string? Description);
