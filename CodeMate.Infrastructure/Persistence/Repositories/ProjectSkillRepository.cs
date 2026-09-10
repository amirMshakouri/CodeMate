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
}