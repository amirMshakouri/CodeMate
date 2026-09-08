using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Contracts.Auth.Responses
{
    public sealed class ResetPasswordResponse
    {
        public required string Message { get; set; }
    }
}