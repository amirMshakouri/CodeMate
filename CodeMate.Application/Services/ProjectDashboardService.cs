using System;
using System.Collections.Generic;
using System.Text;

using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Dashboard.Responses;
using CodeMate.Shared.Exceptions;
using TaskStatusEnum = CodeMate.Domain.Enums.TaskStatus;

namespace CodeMate.Application.Services;

public sealed class ProjectDashboardService : IProjectDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;

    public ProjectDashboardService(
        IDashboardRepository dashboardRepository,
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _dashboardRepository = dashboardRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ProjectDashboardResponse> GetProjectStatsAsync(Guid projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
            throw new NotFoundException("Project not found.");

        if (project.OwnerId != _currentUserService.UserId &&
      _currentUserService.Role != CodeMate.Domain.Enums.UserRole.Admin)
            throw new ForbiddenException("You are not the owner of this project.");

        var tasks = await _dashboardRepository.GetProjectTasksAsync(projectId);

        var total = tasks.Count;
        var done = tasks.Count(t => t.Status == TaskStatusEnum.Done);

        var memberPerformance = new List<MemberPerformanceResponse>();

        foreach (var group in tasks
            .Where(t => t.Status == TaskStatusEnum.Done && t.AssignedUserId.HasValue)
            .GroupBy(t => t.AssignedUserId!.Value))
        {
            var user = await _userRepository.GetByIdAsync(group.Key);

            memberPerformance.Add(new MemberPerformanceResponse
            {
                UserId = group.Key,
                UserName = user?.UserName ?? string.Empty,
                CompletedTasksCount = group.Count()
            });
        }

        return new ProjectDashboardResponse
        {
            TotalTasks = total,
            TodoCount = tasks.Count(t => t.Status == TaskStatusEnum.Todo),
            DoingCount = tasks.Count(t => t.Status == TaskStatusEnum.Doing),
            DoneCount = done,
            OverdueCount = tasks.Count(t =>
                t.DueDate.HasValue &&
                t.DueDate.Value < DateTimeOffset.UtcNow &&
                t.Status != TaskStatusEnum.Done),
            ProgressPercentage = total == 0 ? 0 : Math.Round((double)done / total * 100, 2),
            MemberPerformance = memberPerformance
        };
    }
}