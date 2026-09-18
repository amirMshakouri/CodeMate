using CodeMate.Domain.Entities;

namespace CodeMate.Application.Common.Interfaces.Repositories
{
    public interface ISkillRepository
    {
        Task<Skill?> GetByNameAsync(string name);

        Task<IEnumerable<Skill>> SearchAsync(string? term);

        Task AddSkillAsync(Skill skill);

        Task<IEnumerable<UserSkill>> GetUserSkillsAsync(Guid userId);

        Task AddUserSkillAsync(UserSkill userSkill);

        Task RemoveUserSkillAsync(Guid userId, Guid skillId);

        Task<bool> HasUserSkillAsync(Guid userId, Guid skillId);

        Task SaveChangesAsync();
        Task<Skill?> GetByIdAsync(Guid id);

        Task UpdateAsync(Skill skill);

        Task DeleteAsync(Guid id);


    }
}