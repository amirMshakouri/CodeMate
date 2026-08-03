using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Contracts.Auth.Responses
{
    public sealed class RegisterResponse
    {
        public Guid Id { get; set; }

        public required string UserName { get; set; }
        public required string Email { get; set; }

        public required string Message { get; set; }
    }
}
