using CodeMate.Domain.Entities;

namespace CodeMate.Application.Common.Interfaces.Repositories;

public interface IProjectSkillRepository
{
    Task<ProjectSkill?> GetByIdAsync(Guid id);
}