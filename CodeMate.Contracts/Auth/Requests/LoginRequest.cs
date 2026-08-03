using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Contracts.Auth.Requests
{
    public sealed class LoginRequest
    {
        public required string UserNameOrEmail { get; set; }

        public required string Password { get; set; } 
    }
}
