using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NutriView.API.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// The authenticated caller's id, read from the token's "sub" claim.
        /// Only valid inside an endpoint protected by [Authorize].
        /// </summary>
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!Guid.TryParse(value, out var userId))
                throw new InvalidOperationException("The token does not carry a valid user id");

            return userId;
        }
    }
}
