using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Contracts.Users.Responses
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
