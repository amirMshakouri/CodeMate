using System;
using CodeMate.Domain.Common.Base;

namespace CodeMate.Domain.Entities
{
    public class User: BaseEntity
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string SecondaryPassword { get; set; } = string.Empty;
        
        public string? FullName { get; set; } 
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }

    }
}
