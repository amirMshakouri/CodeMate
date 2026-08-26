using CodeMate.Domain.Common.Base;
using CodeMate.Domain.Enums;

namespace CodeMate.Domain.Entities
{
    public class Project : BaseEntity
    {
        public Guid OwnerId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ProjectStatus Status { get; set; }

        public User Owner { get; set; } = null!;
    }
}