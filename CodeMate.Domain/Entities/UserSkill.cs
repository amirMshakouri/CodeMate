using CodeMate.Domain.Common.Base;
using CodeMate.Domain.Enums;

namespace CodeMate.Domain.Entities
{
    public class UserSkill : BaseEntity
    {
        public Guid UserId { get; set; }

        public Guid SkillId { get; set; }

        public SkillLevel Level { get; set; }

        public int? YearsOfExperience { get; set; }

        public User User { get; set; } = null!;

        public Skill Skill { get; set; } = null!;
    }
}