using CodeMate.Contracts.Teams.Enums;

namespace CodeMate.Contracts.Teams.Responses;

public sealed class JoinRequestResponse
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string ProjectTitle { get; set; } = string.Empty;
    public string? Message { get; set; }
    public JoinRequestStatusResponse Status { get; set; }
}