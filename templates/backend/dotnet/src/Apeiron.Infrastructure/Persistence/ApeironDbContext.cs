using Apeiron.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Apeiron.Infrastructure.Persistence;

public class ApeironDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public ApeironDbContext(DbContextOptions<ApeironDbContext> options) : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApeironDbContext).Assembly);
    }
}