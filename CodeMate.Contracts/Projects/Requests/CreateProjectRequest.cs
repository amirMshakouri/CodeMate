namespace CodeMate.Contracts.Projects.Requests;

public sealed class CreateProjectRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
}