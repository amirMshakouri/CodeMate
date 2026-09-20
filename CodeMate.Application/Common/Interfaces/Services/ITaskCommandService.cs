
using CodeMate.Contracts.Tasks.Requests;
using CodeMate.Contracts.Tasks.Responses;

namespace CodeMate.Application.Common.Interfaces.Services;

public interface ITaskCommandService
{
    Task<TaskResponse> CreateAsync(
        CreateTaskRequest request);

    Task<TaskResponse> UpdateAsync(
        Guid taskId,
        UpdateTaskRequest request);

    Task DeleteAsync(
        Guid taskId);

    Task<TaskResponse> AssignAsync(
        Guid taskId,
        AssignTaskRequest request);

    Task<TaskResponse> ChangeStatusAsync(
        Guid taskId,
        ChangeTaskStatusRequest request);
}

