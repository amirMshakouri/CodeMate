using CodeMate.Contracts.Auth.Requests;
using CodeMate.Contracts.Auth.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMate.Application.Common.Interfaces.Services
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);

        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
