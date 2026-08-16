using CodeMate.Contracts.Skills.Requests;
using CodeMate.Contracts.Skills.Responses;

namespace CodeMate.Application.Common.Interfaces.Services
{
    public interface IUserSkillService
    {
        Task<IEnumerable<SkillResponse>> GetMySkillsAsync(Guid userId);

        Task<SkillResponse> AddSkillAsync(
            Guid userId,
            AddUserSkillRequest request);

        Task RemoveSkillAsync(
            Guid userId,
            Guid skillId);
    }
}