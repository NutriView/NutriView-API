using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NutriView.API.Configuration;
using NutriView.API.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NutriView.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _settings;

        public TokenService(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }

        public TokenResult CreateToken(User user)
        {
            var expiresAt = DateTime.UtcNow.AddDays(_settings.ExpiryDays);

            // "sub" carries the user id: every protected endpoint resolves the caller
            // from this claim instead of trusting an id supplied by the client.
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new TokenResult(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }
    }
}
