

using Apeiron.Application.Interfaces.Projects;
using Apeiron.Application.Interfaces.Users;
using Apeiron.Application.Services.Projects;
using Apeiron.Application.Services.Users;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Apeiron.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IUserService, UserService>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
