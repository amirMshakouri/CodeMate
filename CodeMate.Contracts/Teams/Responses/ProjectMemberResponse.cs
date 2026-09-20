using CodeMate.Contracts.Teams.Enums;

namespace CodeMate.Contracts.Teams.Responses;

public sealed class ProjectMemberResponse
{
    public Guid TeamId { get; set; }

    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public TeamMemberRoleResponse Role { get; set; }

    public DateTimeOffset JoinedAt { get; set; }

    public bool IsActive { get; set; }
}