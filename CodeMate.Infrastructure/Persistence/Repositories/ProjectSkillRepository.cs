using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Domain.Entities;
using CodeMate.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CodeMate.Infrastructure.Persistence.Repositories;

public partial class ProjectSkillRepository : IProjectSkillRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectSkillRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectSkill?> GetByIdAsync(Guid id)
    {
        return await _context.ProjectSkills
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    public async Task<IEnumerable<ProjectSkill>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.ProjectSkills
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId && !x.IsDeleted)
            .ToListAsync();
    }
    
    public async Task AddAsync(ProjectSkill projectSkill)
    {
        await _context.ProjectSkills.AddAsync(projectSkill);
    }

    public Task DeleteAsync(ProjectSkill projectSkill)
    {
        _context.ProjectSkills.Update(projectSkill);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}