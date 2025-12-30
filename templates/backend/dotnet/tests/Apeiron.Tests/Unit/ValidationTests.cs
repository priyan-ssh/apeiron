using Apeiron.Application.Contracts.Projects;
using FluentAssertions;

namespace Apeiron.Tests.Unit;

public class ValidationTests
{
    [Theory]
    [InlineData("")]
    [InlineData((string)null)]
    public void ProjectCreateValidator_ShouldFail_WhenNameIsMissing(string name)
    {
        // Arrange
        var validator = new ProjectCreateValidator();
        var request = new ProjectCreateRequest(name, "Description");

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void ProjectCreateValidator_ShouldPass_WhenRequestIsValid()
    {
        // Arrange
        var validator = new ProjectCreateValidator();
        var request = new ProjectCreateRequest("Valid Name", "Valid Description");

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
