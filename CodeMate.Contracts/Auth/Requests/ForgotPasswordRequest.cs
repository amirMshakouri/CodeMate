using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Contracts.Auth.Requests
{
    public sealed class ForgotPasswordRequest
    {
        public required string Email { get; set; }
    }
}