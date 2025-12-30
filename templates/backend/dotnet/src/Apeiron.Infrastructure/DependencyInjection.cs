using Apeiron.Application.Interfaces.Projects;
using Apeiron.Application.Common.Models;
using Apeiron.Application.Interfaces.Users;
using Apeiron.Application.Interfaces.Auth;
using Apeiron.Infrastructure.Persistence;
using Apeiron.Infrastructure.Repositories.Projects;
using Apeiron.Infrastructure.Repositories.Users;
using Apeiron.Infrastructure.Identity;
using Apeiron.Infrastructure.Persistence.Interceptors;
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

        services.AddScoped<AuditingInterceptor>();
        services.AddDbContext<ApeironDbContext>((sp, options) => {
            var interceptor = sp.GetRequiredService<AuditingInterceptor>();
            options.UseNpgsql(connectionString)
                   .AddInterceptors(interceptor);
        });

        // Register Repositories
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // Register Identity
        services.AddIdentityCore<User>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApeironDbContext>();
            
        // Register HybridCache (.NET 9)
        // Register HybridCache (.NET 9)
        var featureFlags = configuration.GetSection(FeatureFlags.SectionName).Get<FeatureFlags>();
        if (featureFlags?.EnableCaching == true)
        {
            services.AddHybridCache();
        }

        return services;
    }
}