using CodeMate.Domain.Enums;

namespace CodeMate.Contracts.Projects.Responses;

public sealed class ProjectResponse
{
    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ProjectStatus Status { get; set; }
}