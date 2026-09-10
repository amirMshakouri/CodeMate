using CodeMate.Contracts.Teams.Requests;
using CodeMate.Contracts.Teams.Responses;

namespace CodeMate.Application.Common.Interfaces.Services;

public interface ITeamCommandService
{
    Task<TeamResponse> CreateAsync(
        CreateTeamRequest request);

    Task<TeamResponse> UpdateAsync(
        Guid teamId,
        UpdateTeamRequest request);

    Task DeleteAsync(
        Guid teamId);
}