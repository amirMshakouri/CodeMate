using CodeMate.Domain.Common.Base;
using CodeMate.Domain.Enums;
using TaskStatusEnum = CodeMate.Domain.Enums.TaskStatus;

namespace CodeMate.Domain.Entities;

public class TaskItem : BaseEntity
{
    public Guid ProjectId { get; set; }

    public Guid? AssignedUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskStatusEnum Status { get; set; } = TaskStatusEnum.Todo;

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTimeOffset? DueDate { get; set; }

    public Project Project { get; set; } = null!;

    public User? AssignedUser { get; set; }
}