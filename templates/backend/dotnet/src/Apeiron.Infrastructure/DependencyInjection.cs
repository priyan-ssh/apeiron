using Apeiron.Application.Interfaces.Projects;
using Apeiron.Application.Interfaces.Users;
using Apeiron.Infrastructure.Persistence;
using Apeiron.Infrastructure.Repositories.Projects;
using Apeiron.Infrastructure.Repositories.Users;
using Apeiron.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Apeiron.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApeironDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Register Repositories
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Register Identity
        services.AddIdentityCore<User>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApeironDbContext>();

        return services;
    }
}