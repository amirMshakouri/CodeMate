using CodeMate.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeMate.API.Controllers;

[ApiController]
[Route("api/projects")]
public partial class ProjectsController : ControllerBase
{
    private readonly IProjectCommandService _projectCommandService;

    public ProjectsController(
        IProjectCommandService projectCommandService)
    {
        _projectCommandService = projectCommandService;
    }
}