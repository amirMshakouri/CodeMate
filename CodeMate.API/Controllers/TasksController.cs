using CodeMate.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeMate.API.Controllers;

[ApiController]
[Route("api/tasks")]
public partial class TasksController : ControllerBase
{
    private readonly ITaskQueryService _taskQueryService;

    public TasksController(ITaskQueryService taskQueryService)
    {
        _taskQueryService = taskQueryService;
    }
}