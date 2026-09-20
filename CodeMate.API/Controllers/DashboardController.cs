using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Dashboard.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CodeMate.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public partial class DashboardController : ControllerBase
{
    private readonly IProjectDashboardService _projectDashboardService;
    private readonly IMyDashboardService _myDashboardService;

    public DashboardController(
        IProjectDashboardService projectDashboardService,
        IMyDashboardService myDashboardService)
    {
        _projectDashboardService = projectDashboardService;
        _myDashboardService = myDashboardService;
    }

    [HttpGet("projects/{projectId:guid}")]
    public async Task<ActionResult<ProjectDashboardResponse>> GetProjectDashboard(Guid projectId)
    {
        var result = await _projectDashboardService.GetProjectStatsAsync(projectId);
        return Ok(result);
    }
}