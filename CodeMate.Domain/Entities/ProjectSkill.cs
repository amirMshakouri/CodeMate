using CodeMate.Domain.Common.Base;
using CodeMate.Domain.Enums;

namespace CodeMate.Domain.Entities;

public class ProjectSkill : BaseEntity
{
    public Guid ProjectId { get; set; }

    public Guid SkillId { get; set; }

    public SkillLevel RequiredLevel { get; set; }

    public bool IsMandatory { get; set; }
}