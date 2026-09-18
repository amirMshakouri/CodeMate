using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Domain.Enums;
using CodeMate.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CodeMate.Infrastructure.Authentication.CurrentUser;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userId, out var id)
                ? id
                : Guid.Empty;
        }
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?
            .User
            .Identity?
            .IsAuthenticated ?? false;
    public UserRole Role
    {
        get
        {
            var role = _httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(ClaimTypes.Role);

            return Enum.TryParse<UserRole>(role, out var parsedRole)
                ? parsedRole
                : UserRole.User;
        }
    }
}