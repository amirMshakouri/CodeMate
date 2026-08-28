using CodeMate.Contracts.Projects.Enums;

namespace CodeMate.Contracts.Projects.Responses;

public class ProjectDetailsResponse
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectStatusResponse Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}