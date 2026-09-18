
using CodeMate.Contracts.Tasks.Requests;
using CodeMate.Contracts.Tasks.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CodeMate.API.Controllers;

public partial class TasksController
{
    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(
        CreateTaskRequest request)
    {
        var result = await _taskCommandService.CreateAsync(request);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> Update(
        Guid id,
        UpdateTaskRequest request)
    {
        var result = await _taskCommandService.UpdateAsync(
            id,
            request);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _taskCommandService.DeleteAsync(id);

        return NoContent();
    }

    [HttpPatch("{id:guid}/assign")]
    public async Task<ActionResult<TaskResponse>> Assign(
        Guid id,
        AssignTaskRequest request)
    {
        var result = await _taskCommandService.AssignAsync(
            id,
            request);

        return Ok(result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<TaskResponse>> ChangeStatus(
        Guid id,
        ChangeTaskStatusRequest request)
    {
        var result = await _taskCommandService.ChangeStatusAsync(
            id,
            request);

        return Ok(result);
    }
}

