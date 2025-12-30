using Apeiron.Application.Common.Models;
using Apeiron.Application.Contracts.Projects;

namespace Apeiron.Application.Interfaces.Projects;

public interface IProjectService
{
    Task<Result<List<ProjectResponse>>> GetAllAsync();
    Task<Result<ProjectResponse>> GetByIdAsync(Guid id);
    Task<Result<ProjectResponse>> CreateAsync(ProjectCreateRequest request);
    Task<Result> UpdateAsync(Guid id, ProjectCreateRequest request);
    Task<Result> DeleteAsync(Guid id);
}
