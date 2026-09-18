using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Contracts.Dashboard.Responses;

public sealed class ProjectDashboardResponse
{
    public int TotalTasks { get; set; }
    public int TodoCount { get; set; }
    public int DoingCount { get; set; }
    public int DoneCount { get; set; }
    public int OverdueCount { get; set; }
    public double ProgressPercentage { get; set; }
    public IEnumerable<MemberPerformanceResponse> MemberPerformance { get; set; } = [];
}