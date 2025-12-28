using Apeiron.Domain.Entities;
using Apeiron.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Apeiron.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<List<Project>> GetAllAsync() => await _projectRepository.Query().ToListAsync();

    public async Task<Project?> GetByIdAsync(Guid id) => await _projectRepository.GetByIdAsync(id);

    public async Task AddAsync(Project project) => await _projectRepository.AddAsync(project);

    public async Task UpdateAsync(Project project) => await _projectRepository.UpdateAsync(project);

    public async Task DeleteAsync(Project project) => await _projectRepository.DeleteAsync(project);
}