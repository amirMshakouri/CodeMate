using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Contracts.Auth.Responses
{
    public sealed class ForgotPasswordResponse
    {
        public required string Message { get; set; }
    }
}