using CodeMate.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeMate.API.Controllers;

[ApiController]
[Route("api/projects")]
public partial class ProjectsController : ControllerBase
{
    private readonly IProjectCommandService _projectCommandService;
    private readonly IProjectQueryService _projectQueryService;

    public ProjectsController(
        IProjectCommandService projectCommandService,
        IProjectQueryService projectQueryService)
    {
        _projectCommandService = projectCommandService;
        _projectQueryService = projectQueryService;
    }
}