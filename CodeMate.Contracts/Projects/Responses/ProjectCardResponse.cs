using CodeMate.Contracts.Projects.Enums;

namespace CodeMate.Contracts.Projects.Responses;

public class ProjectCardResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ProjectStatusResponse Status { get; set; }
}