using Apeiron.Application.Interfaces.Projects;
using Apeiron.Domain.Entities;
using Apeiron.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Apeiron.Infrastructure.Repositories.Projects;

public class ProjectRepository : IProjectRepository
{
    private readonly ApeironDbContext _context;

    public ProjectRepository(ApeironDbContext context) => _context = context;

    public async Task<Project?> GetByIdAsync(Guid id) => await _context.Projects.FindAsync(id);
    
    public async Task<List<Project>> GetAllAsync() => await _context.Projects.ToListAsync();

    public IQueryable<Project> Query() => _context.Projects.AsQueryable();

    public async Task AddAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Project project)
    {
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
    }
}