using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Application.Common.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string resetToken, DateTimeOffset expiresAt);
    }
}
