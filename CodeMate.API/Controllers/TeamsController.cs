using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Teams.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CodeMate.API.Controllers;

[ApiController]
[Route("api/teams")]
public partial class TeamsController : ControllerBase
{
    private readonly ITeamCommandService _teamCommandService;
    private readonly IJoinRequestCommandService _joinRequestCommandService;
    private readonly ITeamQueryService _teamQueryService;
    private readonly IJoinRequestQueryService _joinRequestQueryService;

    public TeamsController(
        ITeamCommandService teamCommandService,
        IJoinRequestCommandService joinRequestCommandService,
        ITeamQueryService teamQueryService,
        IJoinRequestQueryService joinRequestQueryService)
    {
        _teamCommandService = teamCommandService;
        _joinRequestCommandService = joinRequestCommandService;
        _teamQueryService = teamQueryService;
        _joinRequestQueryService = joinRequestQueryService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTeamRequest request)
    {
        var result = await _teamCommandService.CreateAsync(request);

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateTeamRequest request)
    {
        var result = await _teamCommandService.UpdateAsync(request.Id, request);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _teamCommandService.DeleteAsync(id);

        return NoContent();
    }

    [HttpPost("{id:guid}/join-requests")]
    public async Task<IActionResult> SendJoinRequest(
        Guid id,
        SendJoinRequest request)
    {
        request.TeamId = id;

        await _joinRequestCommandService.SendAsync(request);

        return Ok();
    }

    [HttpPatch("{id:guid}/join-requests/{requestId:guid}/accept")]
    public async Task<IActionResult> AcceptJoinRequest(
        Guid id,
        Guid requestId)
    {
        await _joinRequestCommandService.AcceptAsync(
            new AcceptJoinRequest
            {
                RequestId = requestId
            });

        return NoContent();
    }

    [HttpPatch("{id:guid}/join-requests/{requestId:guid}/reject")]
    public async Task<IActionResult> RejectJoinRequest(
        Guid id,
        Guid requestId)
    {
        await _joinRequestCommandService.RejectAsync(
            new RejectJoinRequest
            {
                RequestId = requestId
            });

        return NoContent();
    }

    [HttpDelete("{id:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember(
        Guid id,
        Guid userId)
    {
        await _joinRequestCommandService.RemoveMemberAsync(id, userId);

        return NoContent();
    }
}