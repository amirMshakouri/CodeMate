using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Projects.Responses;
using CodeMate.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeMate.API.Controllers;

[ApiController]
[Route("api/admin/projects")]
[Authorize(Roles = "Admin")]
public class AdminProjectsController : ControllerBase
{
    private readonly IProjectCommandService _projectCommandService;

    public AdminProjectsController(IProjectCommandService projectCommandService)
    {
        _projectCommandService = projectCommandService;
    }

    [HttpPatch("{id:guid}/force-status")]
    public async Task<ActionResult<ProjectResponse>> ForceStatus(
        Guid id,
        [FromQuery] ProjectStatus status)
    {
        var result = await _projectCommandService.ChangeStatusAsync(id, status);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _projectCommandService.DeleteAsync(id);

        return NoContent();
    }
}