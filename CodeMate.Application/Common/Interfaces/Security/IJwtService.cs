using CodeMate.Contracts.Auth.Responses;
using CodeMate.Domain.Entities;

namespace CodeMate.Application.Common.Interfaces.Security;

public interface IJwtService
{
    JwtTokenResult GenerateToken(User user);
}