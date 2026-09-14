using CodeMate.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using CodeMate.Contracts.Teams.Requests;

namespace CodeMate.API.Controllers;

[ApiController]
[Route("api/projects")]
public partial class ProjectsController : ControllerBase
{
    private readonly IProjectCommandService _projectCommandService;
    private readonly IProjectQueryService _projectQueryService;
    private readonly IProjectSkillCommandService _projectSkillCommandService;
    
    public ProjectsController(
        IProjectCommandService projectCommandService,
        IProjectQueryService projectQueryService,
        IProjectSkillCommandService projectSkillCommandService)
    {
        _projectCommandService = projectCommandService;
        _projectQueryService = projectQueryService;
        _projectSkillCommandService = projectSkillCommandService;
    }
    
    [HttpPost("{id:guid}/skills")]
    public async Task<IActionResult> AddProjectSkill(
        Guid id,
        AddProjectSkillRequest request)
    {
        request.ProjectId = id;

        await _projectSkillCommandService.AddAsync(request);

        return NoContent();
    }
    
    [HttpDelete("{id:guid}/skills")]
    public async Task<IActionResult> RemoveProjectSkill(
        Guid id,
        Guid skillId)
    {
        await _projectSkillCommandService.RemoveAsync(id, skillId);

        return NoContent();
    }
}