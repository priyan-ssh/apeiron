using System.Reflection;
using Apeiron.Domain.Entities;
using FluentAssertions;

namespace Apeiron.Tests.Architecture;

public class ArchitectureTests
{
    [Fact]
    public void Domain_ShouldNotDependOn_ApplicationOrInfrastructure()
    {
        // Arrange
        var domainAssembly = typeof(BaseEntity).Assembly;
        
        // Act
        var referencedAssemblies = domainAssembly.GetReferencedAssemblies();

        // Assert
        referencedAssemblies.Should().NotContain(a => a.Name!.Contains("Apeiron.Application"));
        referencedAssemblies.Should().NotContain(a => a.Name!.Contains("Apeiron.Infrastructure"));
        referencedAssemblies.Should().NotContain(a => a.Name!.Contains("Apeiron.Api"));
    }

    [Fact]
    public void Application_ShouldNotDependOn_Infrastructure()
    {
        // Arrange  
        // Note: Application depends on Domain, but NOT Infrastructure.
        var appAssembly = typeof(Apeiron.Application.DependencyInjection).Assembly;

        // Act
        var referencedAssemblies = appAssembly.GetReferencedAssemblies();

        // Assert
        referencedAssemblies.Should().NotContain(a => a.Name!.Contains("Apeiron.Infrastructure"));
    }
}
