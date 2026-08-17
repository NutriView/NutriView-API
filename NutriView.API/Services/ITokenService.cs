using NutriView.API.Models.Entities;

namespace NutriView.API.Services
{
    /// <summary>A signed access token and the moment it stops being accepted.</summary>
    public record TokenResult(string Token, DateTime ExpiresAt);

    public interface ITokenService
    {
        TokenResult CreateToken(User user);
    }
}
