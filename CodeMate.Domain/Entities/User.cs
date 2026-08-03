using CodeMate.Domain.Common.Base;
using CodeMate.Domain.Enums;
using System;

namespace CodeMate.Domain.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string SecondaryPassword { get; set; } = string.Empty;

        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }

        public UserRole Role { get; set; } = UserRole.User;

    }
}
