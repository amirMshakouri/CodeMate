using CodeMate.Contracts.Teams.Responses;

namespace CodeMate.Application.Common.Interfaces.Services;

public interface ITeamQueryService
{
    Task<IEnumerable<TeamCardResponse>> GetTeamsByProjectAsync(
        Guid projectId);

    Task<IEnumerable<ProjectMemberResponse>> GetTeamMembersAsync(
        Guid teamId);
}