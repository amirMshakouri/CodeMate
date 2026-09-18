using CodeMate.Contracts.Common.Pagination;
using CodeMate.Contracts.Tasks.Requests;
using CodeMate.Contracts.Tasks.Responses;

namespace CodeMate.Application.Common.Interfaces.Services;

public interface ITaskQueryService
{
    Task<PaginationResponse<TaskCardResponse>> SearchAsync(
        SearchTaskRequest request);

    Task<IEnumerable<TaskCardResponse>> GetMyTasksAsync();

    Task<TaskDetailsResponse> GetByIdAsync(Guid taskId);
}