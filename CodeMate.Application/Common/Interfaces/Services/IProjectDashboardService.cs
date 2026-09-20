using System;
using System.Collections.Generic;
using System.Text;
using CodeMate.Contracts.Dashboard.Responses;

namespace CodeMate.Application.Common.Interfaces.Services;

public interface IProjectDashboardService
{
    Task<ProjectDashboardResponse> GetProjectStatsAsync(Guid projectId);
}