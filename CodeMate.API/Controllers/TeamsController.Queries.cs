using CodeMate.Contracts.Teams.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CodeMate.API.Controllers;

public partial class TeamsController
{
    [HttpGet("{id:guid}/members")]
    public async Task<ActionResult<IEnumerable<ProjectMemberResponse>>> GetMembers(
        Guid id)
    {
        var result = await _teamQueryService.GetTeamMembersAsync(id);

        return Ok(result);
    }

    [HttpGet("{id:guid}/join-requests")]
    public async Task<ActionResult<IEnumerable<JoinRequestResponse>>>
        GetPendingJoinRequests(Guid id)
    {
        var result =
            await _joinRequestQueryService.GetPendingRequestsForTeamAsync(id);

        return Ok(result);
    }

    [HttpGet("~/api/projects/{id:guid}/teams")]
    public async Task<ActionResult<IEnumerable<TeamCardResponse>>>
        GetTeamsByProject(Guid id)
    {
        var result = await _teamQueryService.GetTeamsByProjectAsync(id);

        return Ok(result);
    }
}