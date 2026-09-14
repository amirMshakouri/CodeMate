using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Teams.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CodeMate.API.Controllers;

[ApiController]
[Route("api/join-requests")]
public sealed class JoinRequestsController : ControllerBase
{
    private readonly IJoinRequestQueryService _joinRequestQueryService;

    public JoinRequestsController(
        IJoinRequestQueryService joinRequestQueryService)
    {
        _joinRequestQueryService = joinRequestQueryService;
    }

    [HttpGet("mine")]
    public async Task<ActionResult<IEnumerable<JoinRequestResponse>>> Mine()
    {
        var result = await _joinRequestQueryService.GetMyJoinRequestsAsync();

        return Ok(result);
    }
}