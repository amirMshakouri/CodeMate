using System;
using System.Collections.Generic;
using System.Text;

using CodeMate.Application.Common.Interfaces.Repositories;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Contracts.Dashboard.Responses;
using TaskStatusEnum = CodeMate.Domain.Enums.TaskStatus;

namespace CodeMate.Application.Services;

public sealed class MyDashboardService : IMyDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly ICurrentUserService _currentUserService;

    public MyDashboardService(
        IDashboardRepository dashboardRepository,
        ICurrentUserService currentUserService)
    {
        _dashboardRepository = dashboardRepository;
        _currentUserService = currentUserService;
    }

    public async Task<MyDashboardResponse> GetMyStatsAsync()
    {
        var userId = _currentUserService.UserId;

        var tasks = await _dashboardRepository.GetMyTasksAsync(userId);
        var projectIds = await _dashboardRepository.GetMyProjectIdsAsync(userId);

        return new MyDashboardResponse
        {
            MyTasksTotal = tasks.Count,
            MyTasksCompleted = tasks.Count(t => t.Status == TaskStatusEnum.Done),
            MyTasksOverdue = tasks.Count(t =>
                t.DueDate.HasValue &&
                t.DueDate.Value < DateTimeOffset.UtcNow &&
                t.Status != TaskStatusEnum.Done),
            MyProjects = projectIds
        };
    }
}