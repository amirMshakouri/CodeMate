using CodeMate.Contracts.Tasks.Enums;

namespace CodeMate.Contracts.Tasks.Requests;

public sealed class ChangeTaskStatusRequest
{
    public TaskStatusResponse Status { get; set; }
}