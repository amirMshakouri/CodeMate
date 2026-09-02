using CodeMate.Contracts.Teams.Responses;

namespace CodeMate.Application.Common.Interfaces.Services;

public interface IJoinRequestQueryService
{
    Task<IEnumerable<JoinRequestResponse>> GetPendingRequestsForTeamAsync(
        Guid teamId);

    Task<IEnumerable<JoinRequestResponse>> GetMyJoinRequestsAsync();
}