
using CodeMate.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeMate.API.Controllers;

[ApiController]
[Route("api/tasks")]
public partial class TasksController : ControllerBase
{
    private readonly ITaskQueryService _taskQueryService;
    private readonly ITaskCommandService _taskCommandService;

    public TasksController(
        ITaskQueryService taskQueryService,
        ITaskCommandService taskCommandService)
    {
        _taskQueryService = taskQueryService;
        _taskCommandService = taskCommandService;
    }
}

