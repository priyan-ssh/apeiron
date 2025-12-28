using Apeiron.Application.Interfaces;
using Apeiron.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Apeiron.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IUserService, UserService>();
        
        return services;
    }
}
