using CodeMate.Domain.Common.Base;

namespace CodeMate.Domain.Entities;

public class Team : BaseEntity
{
    public Guid ProjectId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}