using System.Net;
using System.Net.Http.Json;
using Apeiron.Application.Common.Models;
using Apeiron.Application.Contracts.Projects;
using Apeiron.Application.Interfaces.Projects;
using Apeiron.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Apeiron.Tests.Integration;

public class ProjectsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IProjectService _projectServiceMock;

    public ProjectsControllerTests(WebApplicationFactory<Program> factory)
    {
        _projectServiceMock = Substitute.For<IProjectService>();
        
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Replace the real service with our mock
                services.AddScoped(_ => _projectServiceMock);
            });
        });
    }

    [Fact]
    public async Task Get_ShouldReturnOk_WhenProjectExists()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var responseDto = new ProjectResponse(projectId, "Test Project", "Desc", DateTime.UtcNow);
        
        _projectServiceMock.GetByIdAsync(projectId)
            .Returns(Result<ProjectResponse>.Success(responseDto));

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/v1/projects/{projectId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProjectResponse>();
        result.Should().BeEquivalentTo(responseDto);
    }

    [Fact]
    public async Task Create_ShouldReturn400_WhenValidationFails()
    {
        // Arrange
        var client = _factory.CreateClient();
        var invalidRequest = new ProjectCreateRequest("", ""); // Empty name
        
        // Mock the service to throw validation exception (simulating Service behavior)
        _projectServiceMock.CreateAsync(Arg.Any<ProjectCreateRequest>())
            .Returns(Task.FromException<Result<ProjectResponse>>(new ValidationException("Validation Failed", new[] { new ValidationFailure("Name", "Name is required") })));

        // Act
        // Note: The ExceptionHandler middleware intercepts this
        var response = await client.PostAsJsonAsync("/api/v1/projects", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
