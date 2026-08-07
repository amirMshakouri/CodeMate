using CodeMate.Domain.Entities;

namespace CodeMate.Application.Common.Interfaces.Security
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}