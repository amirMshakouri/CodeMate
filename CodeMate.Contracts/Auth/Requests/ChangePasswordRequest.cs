using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Contracts.Auth.Requests
{
    public sealed class ChangePasswordRequest
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmNewPassword { get; set; }
    }
}