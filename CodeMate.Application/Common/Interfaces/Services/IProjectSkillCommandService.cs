using CodeMate.Contracts.Teams.Requests;

namespace CodeMate.Application.Common.Interfaces.Services;

public interface IProjectSkillCommandService
{
    Task AddAsync(
        AddProjectSkillRequest request);

    Task RemoveAsync(
        Guid projectId,
        Guid skillId);
}