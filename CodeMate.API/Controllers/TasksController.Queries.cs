using CodeMate.Contracts.Common.Pagination;
using CodeMate.Contracts.Tasks.Requests;
using CodeMate.Contracts.Tasks.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CodeMate.API.Controllers;

public partial class TasksController
{
    [HttpGet]
    public async Task<ActionResult<PaginationResponse<TaskCardResponse>>> Search(
        [FromQuery] SearchTaskRequest request)
    {
        var result = await _taskQueryService.SearchAsync(request);

        return Ok(result);
    }

    [HttpGet("mine")]
    public async Task<ActionResult<IEnumerable<TaskCardResponse>>> GetMyTasks()
    {
        var result = await _taskQueryService.GetMyTasksAsync();

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskDetailsResponse>> GetById(Guid id)
    {
        var result = await _taskQueryService.GetByIdAsync(id);

        return Ok(result);
    }
}