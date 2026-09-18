using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Contracts.Dashboard.Responses;

public sealed class MemberPerformanceResponse
{
    public Guid UserId { get; set; }
    public int CompletedTasksCount { get; set; }
}