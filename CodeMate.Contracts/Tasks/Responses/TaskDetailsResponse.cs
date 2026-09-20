using CodeMate.Contracts.Tasks.Enums;

namespace CodeMate.Contracts.Tasks.Responses;

public class TaskDetailsResponse
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public Guid? AssignedUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskStatusResponse Status { get; set; }

    public TaskPriorityResponse Priority { get; set; }

    public DateTimeOffset? DueDate { get; set; }
}