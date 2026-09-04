using CodeMate.Domain.Entities;

namespace CodeMate.Infrastructure.Persistence.Repositories;

public partial class ProjectRepository
{
    public async Task AddAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
    }

    public Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Project project)
    {
        _context.Projects.Update(project);

        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}