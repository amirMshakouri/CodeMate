using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Contracts.Dashboard.Responses;

public sealed class MyDashboardResponse
{
    public int MyTasksTotal { get; set; }
    public int MyTasksCompleted { get; set; }
    public int MyTasksOverdue { get; set; }
    public IEnumerable<Guid> MyProjects { get; set; } = [];
}