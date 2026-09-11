using CodeMate.Domain.Entities;

namespace CodeMate.Application.Common.Interfaces.Repositories;

public interface IProjectSkillRepository
{
    Task<ProjectSkill?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProjectSkill>> GetByProjectIdAsync(Guid projectId);
    Task AddAsync(ProjectSkill projectSkill);
    Task DeleteAsync(ProjectSkill projectSkill);
    Task SaveChangesAsync();
}