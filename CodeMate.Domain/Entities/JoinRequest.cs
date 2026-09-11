using CodeMate.Domain.Common.Base;
using CodeMate.Domain.Enums;

namespace CodeMate.Domain.Entities;

public class JoinRequest : BaseEntity
{
    public Guid TeamId { get; set; }

    public Guid UserId { get; set; }

    public string? Message { get; set; }

    public JoinRequestStatus Status { get; set; }
}