using CodeMate.Domain.Common.Base;
using CodeMate.Domain.Enums;

namespace CodeMate.Domain.Entities;

public class TeamMember : BaseEntity
{
    public Guid TeamId { get; set; }

    public Guid UserId { get; set; }

    public TeamMemberRole Role { get; set; }

    public DateTimeOffset JoinedAt { get; set; }

    public bool IsActive { get; set; } = true;
}