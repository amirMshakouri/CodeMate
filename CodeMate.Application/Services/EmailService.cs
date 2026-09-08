using System;
using System.Collections.Generic;
using System.Text;

using CodeMate.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CodeMate.Infrastructure.Services
{
    public sealed class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendPasswordResetEmailAsync(string toEmail, string resetToken, DateTimeOffset expiresAt)
        {
            _logger.LogInformation(
                "[DEV EMAIL] To: {Email} | Password reset code: {ResetToken} | Expires at: {ExpiresAt:HH:mm}",
                toEmail, resetToken, expiresAt);

            return Task.CompletedTask;
        }
    }
}