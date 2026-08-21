using System.Security.Claims;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Skills.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeMate.API.Controllers;

[ApiController]
[Route("api/skills")]
public class SkillsController : ControllerBase
{
    private readonly IUserSkillService _userSkillService;

    public SkillsController(IUserSkillService userSkillService)
    {
        _userSkillService = userSkillService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchSkills([FromQuery] string? search)
    {
        var skills = await _userSkillService.SearchSkillsAsync(search);

        return Ok(skills);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMySkills()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var skills = await _userSkillService.GetMySkillsAsync(userId);

        return Ok(skills);
    }

    [HttpPost("me")]
    [Authorize]
    public async Task<IActionResult> AddSkill(
        [FromBody] AddUserSkillRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = await _userSkillService.AddSkillAsync(userId, request);

        return Ok(result);
    }


    [HttpDelete("me/{skillId:guid}")]
    [Authorize]
    public async Task<IActionResult> RemoveSkill(Guid skillId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        await _userSkillService.RemoveSkillAsync(userId, skillId);

        return NoContent();
    }
}