using Apeiron.Application.Contracts.Projects;
using Apeiron.Application.Interfaces.Projects;
using Microsoft.AspNetCore.Mvc;

namespace Apeiron.Api.Controllers.Projects;

public class ProjectsController : BaseApiController
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProjectResponse>>> Get()
    {
        var result = await _projectService.GetAllAsync();
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectResponse>> Get(Guid id)
    {
        var result = await _projectService.GetByIdAsync(id);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create(ProjectCreateRequest request)
    {
        var result = await _projectService.CreateAsync(request);

        return result.IsSuccess 
            ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, ProjectCreateRequest request)
    {
        var result = await _projectService.UpdateAsync(id, request);

        return result.IsSuccess 
            ? NoContent() 
            : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _projectService.DeleteAsync(id);

        return result.IsSuccess 
            ? NoContent() 
            : BadRequest(result.Error);
    }
}
