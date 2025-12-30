using Microsoft.AspNetCore.Identity;

namespace Apeiron.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
