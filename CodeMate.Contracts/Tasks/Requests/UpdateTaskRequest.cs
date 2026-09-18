using CodeMate.Contracts.Tasks.Enums;

namespace CodeMate.Contracts.Tasks.Requests;

public sealed class UpdateTaskRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskPriorityResponse Priority { get; set; }

    public DateTimeOffset? DueDate { get; set; }
}