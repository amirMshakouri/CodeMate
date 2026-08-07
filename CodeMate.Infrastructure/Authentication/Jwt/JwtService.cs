using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CodeMate.Contracts.Auth.Responses;

using CodeMate.Application.Common.Interfaces.Security;
using CodeMate.Domain.Entities;

using Microsoft.IdentityModel.Tokens;

namespace CodeMate.Infrastructure.Authentication.Jwt;

public sealed class JwtService : IJwtService
{
    private readonly JwtSettings _settings;

    public JwtService(JwtSettings settings)
    {
        _settings = settings;
    }

    public JwtTokenResult GenerateToken(User user)
    {
        var expiration = DateTimeOffset.UtcNow.AddMinutes(
            _settings.ExpirationMinutes);

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new Claim(
                ClaimTypes.Name,
                user.UserName),

            new Claim(
                ClaimTypes.Email,
                user.Email),

            new Claim(
                ClaimTypes.Role,
                user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiration.UtcDateTime,
            signingCredentials: credentials
        );

        return new JwtTokenResult
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration
        };
    }
}