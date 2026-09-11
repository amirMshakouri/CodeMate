using CodeMate.Contracts.Teams.Requests;

namespace CodeMate.Application.Common.Interfaces.Services;

public interface IJoinRequestCommandService
{
    Task SendAsync(
        SendJoinRequest request);

    Task AcceptAsync(
        AcceptJoinRequest request);

    Task RejectAsync(
        RejectJoinRequest request);

    Task RemoveMemberAsync(
        Guid teamId,
        Guid userId);
}