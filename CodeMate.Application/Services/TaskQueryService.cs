using AutoMapper;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Common.Pagination;
using CodeMate.Contracts.Tasks.Requests;
using CodeMate.Contracts.Tasks.Responses;
using CodeMate.Shared.Exceptions;
using TaskStatusEnum = CodeMate.Domain.Enums.TaskStatus;

namespace CodeMate.Application.Services;

public sealed class TaskQueryService : ITaskQueryService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public TaskQueryService(
        ITaskRepository taskRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _taskRepository = taskRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<TaskCardResponse>> SearchAsync(
        SearchTaskRequest request)
    {
        var status = request.Status.HasValue
            ? (TaskStatusEnum?)request.Status.Value
            : null;

        var tasks = await _taskRepository.SearchAsync(
            request.ProjectId,
            status,
            request.AssignedUserId,
            request.PageNumber,
            request.PageSize);

        return new PaginationResponse<TaskCardResponse>
        {
            Items = _mapper.Map<IEnumerable<TaskCardResponse>>(tasks),
            PageNumber = tasks.PageNumber,
            PageSize = tasks.PageSize,
            TotalCount = tasks.TotalCount,
            TotalPages = tasks.TotalPages
        };
    }

    public async Task<IEnumerable<TaskCardResponse>> GetMyTasksAsync()
    {
        var tasks = await _taskRepository.GetByAssignedUserIdAsync(
            _currentUserService.UserId);

        return _mapper.Map<IEnumerable<TaskCardResponse>>(tasks);
    }

    public async Task<TaskDetailsResponse> GetByIdAsync(Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task is null)
        {
            throw new NotFoundException("Task not found.");
        }

        return _mapper.Map<TaskDetailsResponse>(task);
    }
}