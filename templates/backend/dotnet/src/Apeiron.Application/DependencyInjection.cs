

using Apeiron.Application.Interfaces.Projects;
using Apeiron.Application.Interfaces.Users;
using Apeiron.Application.Services.Projects;
using Apeiron.Application.Services.Users;
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
