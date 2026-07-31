using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Contracts.Auth.Requests
{
    public sealed class RegisterRequest
    {
        public required string UserName { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public required string ConfirmPassword { get; set; }

        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
    }
}
