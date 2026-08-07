using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CodeMate.Application.Common.Interfaces.Security;
using CodeMate.Domain.Entities;
using Microsoft.IdentityModel.Tokens;


namespace CodeMate.Infrastructure.Authentication.Jwt
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _settings;

        public JwtService(JwtSettings settings)
        {
            _settings = settings;
        }


        public string GenerateToken(User user)
        {
            var claims = new[]
 {
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Name, user.UserName),
    new Claim(ClaimTypes.Email, user.Email),
    new Claim(ClaimTypes.Role, user.Role.ToString())
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
                expires: DateTime.UtcNow.AddMinutes(
                    _settings.ExpirationMinutes),
                signingCredentials: credentials
            );


            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}