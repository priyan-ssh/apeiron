using Apeiron.Application.Common.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace Apeiron.Tests.Unit;

public class FeatureFlagTests
{
    [Fact]
    public void FeatureFlags_ShouldHaveSafeDefaults()
    {
        // Arrange
        var flags = new FeatureFlags();

        // Assert
        flags.EnableAuth.Should().BeTrue("Auth should be enabled by default for safety");
        flags.EnableOpenTelemetry.Should().BeTrue("Observability should be enabled by default");
        flags.EnableCaching.Should().BeFalse("Caching should be disabled by default to prevent complexity");
    }

    [Fact]
    public void FeatureFlags_ShouldBindFromConfig()
    {
        // Arrange
        var myConfig = new Dictionary<string, string>
        {
            {"FeatureFlags:EnableCaching", "true"},
            {"FeatureFlags:EnableAuth", "false"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfig!)
            .Build();

        // Act
        var flags = configuration.GetSection(FeatureFlags.SectionName).Get<FeatureFlags>();

        // Assert
        flags.Should().NotBeNull();
        flags!.EnableCaching.Should().BeTrue();
        flags.EnableAuth.Should().BeFalse();
    }
}
