using Apeiron.Domain.Entities;

namespace Apeiron.Application.Interfaces.Projects;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id);
    Task<List<Project>> GetAllAsync();
    IQueryable<Project> Query();
    Task AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(Project project);
}
