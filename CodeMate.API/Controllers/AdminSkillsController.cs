using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Skills.Requests;
using CodeMate.Contracts.Skills.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeMate.API.Controllers;

[ApiController]
[Route("api/admin/skills")]
[Authorize(Roles = "Admin")]
public class AdminSkillsController : ControllerBase
{
    private readonly IAdminSkillService _adminSkillService;

    public AdminSkillsController(IAdminSkillService adminSkillService)
    {
        _adminSkillService = adminSkillService;
    }

    [HttpPost]
    public async Task<ActionResult<SkillResponse>> Create(CreateSkillRequest request)
    {
        var result = await _adminSkillService.CreateSkillAsync(request);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SkillResponse>> Update(Guid id, UpdateSkillRequest request)
    {
        var result = await _adminSkillService.UpdateSkillAsync(id, request);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _adminSkillService.DeleteSkillAsync(id);

        return NoContent();
    }
}