using AutoMapper;
using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Tasks.Enums;
using CodeMate.Contracts.Tasks.Requests;
using CodeMate.Contracts.Tasks.Responses;
using CodeMate.Domain.Entities;
using CodeMate.Domain.Enums;
using CodeMate.Shared.Exceptions;
using TaskStatusEnum = CodeMate.Domain.Enums.TaskStatus;

namespace CodeMate.Application.Services;

public sealed class TaskCommandService : ITaskCommandService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectMembershipChecker _membershipChecker;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public TaskCommandService(
        ITaskRepository taskRepository,
        IProjectRepository projectRepository,
        IProjectMembershipChecker membershipChecker,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _membershipChecker = membershipChecker;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<TaskResponse> CreateAsync(
        CreateTaskRequest request)
    {
        var project = await GetProjectAsync(request.ProjectId);

        EnsureOwnership(project);
        EnsureProjectNotCompleted(project);

        var task = _mapper.Map<TaskItem>(request);

        task.Status = TaskStatusEnum.Todo;

        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();

        return _mapper.Map<TaskResponse>(task);
    }

    public async Task<TaskResponse> UpdateAsync(
        Guid taskId,
        UpdateTaskRequest request)
    {
        var task = await GetTaskAsync(taskId);

        var project = await GetProjectAsync(task.ProjectId);

        EnsureOwnership(project);
        EnsureProjectNotCompleted(project);

        _mapper.Map(request, task);

        await _taskRepository.UpdateAsync(task);
        await _taskRepository.SaveChangesAsync();

        return _mapper.Map<TaskResponse>(task);
    }

    public async Task DeleteAsync(Guid taskId)
    {
        var task = await GetTaskAsync(taskId);

        var project = await GetProjectAsync(task.ProjectId);

        EnsureOwnership(project);
        EnsureProjectNotCompleted(project);

        task.IsDeleted = true;
        task.DeletedAt = DateTimeOffset.UtcNow;
        task.DeletedBy = _currentUserService.UserId;

        await _taskRepository.DeleteAsync(task);
        await _taskRepository.SaveChangesAsync();
    }

    public async Task<TaskResponse> AssignAsync(
        Guid taskId,
        AssignTaskRequest request)
    {
        var task = await GetTaskAsync(taskId);

        var project = await GetProjectAsync(task.ProjectId);

        EnsureOwnership(project);
        EnsureProjectNotCompleted(project);

        var isMember = await _membershipChecker.IsProjectMemberAsync(
            task.ProjectId,
            request.AssignedUserId);

        if (!isMember)
            throw new ForbiddenException(
                "The assigned user is not a member of the project.");

        task.AssignedUserId = request.AssignedUserId;

        await _taskRepository.UpdateAsync(task);
        await _taskRepository.SaveChangesAsync();

        return _mapper.Map<TaskResponse>(task);
    }

    public async Task<TaskResponse> ChangeStatusAsync(
        Guid taskId,
        ChangeTaskStatusRequest request)
    {
        var task = await GetTaskAsync(taskId);

        var project = await GetProjectAsync(task.ProjectId);

        EnsureStatusPermission(task, project);
        EnsureProjectNotCompleted(project);

        var newStatus = MapStatus(request.Status);

        EnsureValidStatusTransition(
            task.Status,
            newStatus);

        task.Status = newStatus;

        await _taskRepository.UpdateAsync(task);
        await _taskRepository.SaveChangesAsync();

        return _mapper.Map<TaskResponse>(task);
    }

    private async Task<TaskItem> GetTaskAsync(Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task is null)
            throw new NotFoundException("Task not found.");

        return task;
    }

    private async Task<Project> GetProjectAsync(Guid projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
            throw new NotFoundException("Project not found.");

        return project;
    }

    private void EnsureOwnership(Project project)
    {
        if (project.OwnerId != _currentUserService.UserId)
            throw new ForbiddenException(
                "You do not have permission to modify this task.");
    }

    private static void EnsureProjectNotCompleted(Project project)
    {
        if (project.Status == ProjectStatus.Completed)
            throw new ForbiddenException(
                "This project is completed and its tasks can no longer be changed.");
    }

    private void EnsureStatusPermission(
        TaskItem task,
        Project project)
    {
        var currentUserId = _currentUserService.UserId;

        var isOwner =
            project.OwnerId == currentUserId;

        var isAssignedUser =
            task.AssignedUserId.HasValue &&
            task.AssignedUserId.Value == currentUserId;

        if (!isOwner && !isAssignedUser)
            throw new ForbiddenException(
                "You do not have permission to change this task status.");
    }

    private static TaskStatusEnum MapStatus(
        TaskStatusResponse status)
    {
        return status switch
        {
            TaskStatusResponse.Todo => TaskStatusEnum.Todo,
            TaskStatusResponse.Doing => TaskStatusEnum.Doing,
            TaskStatusResponse.Done => TaskStatusEnum.Done,
            _ => throw new ForbiddenException(
                "Invalid task status.")
        };
    }

    private static void EnsureValidStatusTransition(
        TaskStatusEnum currentStatus,
        TaskStatusEnum newStatus)
    {
        var isValid =
            (currentStatus == TaskStatusEnum.Todo &&
             newStatus == TaskStatusEnum.Doing)
            ||
            (currentStatus == TaskStatusEnum.Doing &&
             newStatus == TaskStatusEnum.Done);

        if (!isValid)
            throw new ForbiddenException(
                "Invalid task status transition.");
    }
}
