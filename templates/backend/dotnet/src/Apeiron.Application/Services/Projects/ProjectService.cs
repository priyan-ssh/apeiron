using Apeiron.Application.Common.Models;
using FluentValidation;
using Apeiron.Application.Contracts.Projects;
using Apeiron.Application.Interfaces.Projects;
using Apeiron.Domain.Entities;

namespace Apeiron.Application.Services.Projects;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IValidator<ProjectCreateRequest> _validator;

    public ProjectService(IProjectRepository projectRepository, IValidator<ProjectCreateRequest> validator)
    {
        _projectRepository = projectRepository;
        _validator = validator;
    }

    public async Task<Result<List<ProjectResponse>>> GetAllAsync()
    {
        var projects = await _projectRepository.GetAllAsync();
        
        var response = projects.Select(p => new ProjectResponse(
            p.Id, 
            p.Name, 
            p.Description, 
            p.CreatedAt)).ToList();

        return Result<List<ProjectResponse>>.Success(response);
    }

    public async Task<Result<ProjectResponse>> GetByIdAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        if (project is null)
        {
            return Result<ProjectResponse>.Failure(new Error("Project.NotFound", $"Project with ID {id} not found."));
        }

        var response = new ProjectResponse(project.Id, project.Name, project.Description, project.CreatedAt);
        return Result<ProjectResponse>.Success(response);
    }

    public async Task<Result<ProjectResponse>> CreateAsync(ProjectCreateRequest request)
    {
        _validator.ValidateAndThrow(request);

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _projectRepository.AddAsync(project);

        var response = new ProjectResponse(project.Id, project.Name, project.Description, project.CreatedAt);
        return Result<ProjectResponse>.Success(response);
    }

    public async Task<Result> UpdateAsync(Guid id, ProjectCreateRequest request)
    {
        _validator.ValidateAndThrow(request);
        
        var project = await _projectRepository.GetByIdAsync(id);

        if (project is null)
        {
            return Result.Failure(new Error("Project.NotFound", $"Project with ID {id} not found."));
        }

        project.Name = request.Name;
        project.Description = request.Description;

        await _projectRepository.UpdateAsync(project);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        if (project is null)
        {
            return Result.Failure(new Error("Project.NotFound", $"Project with ID {id} not found."));
        }

        await _projectRepository.DeleteAsync(project);
        return Result.Success();
    }
}