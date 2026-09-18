using System;
using System.Collections.Generic;
using System.Text;

using CodeMate.Domain.Entities;

namespace CodeMate.Application.Common.Interfaces.Repositories;

public interface IDashboardRepository
{
    Task<List<TaskItem>> GetProjectTasksAsync(Guid projectId);

    Task<List<TaskItem>> GetMyTasksAsync(Guid userId);

    Task<List<Guid>> GetMyProjectIdsAsync(Guid userId);
}