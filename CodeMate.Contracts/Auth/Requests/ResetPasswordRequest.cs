using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Contracts.Auth.Requests
{
    public sealed class ResetPasswordRequest
    {
        public required string Email { get; set; }
        public required string Token { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmNewPassword { get; set; }
    }
}