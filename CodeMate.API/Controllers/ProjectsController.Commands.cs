using CodeMate.Contracts.Projects.Requests;
using CodeMate.Contracts.Projects.Responses;
using CodeMate.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CodeMate.API.Controllers;

public partial class ProjectsController
{
    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create(
        CreateProjectRequest request)
    {
        var result = await _projectCommandService.CreateAsync(request);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProjectResponse>> Update(
        Guid id,
        UpdateProjectRequest request)
    {
        var result = await _projectCommandService.UpdateAsync(
            id,
            request);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _projectCommandService.DeleteAsync(id);

        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<ProjectResponse>> ChangeStatus(
        Guid id,
        [FromQuery] ProjectStatus status)
    {
        var result = await _projectCommandService.ChangeStatusAsync(
            id,
            status);

        return Ok(result);
    }
}