namespace NutriView.API.Models.DTOs
{
    /// <summary>What register and login return: the access token plus the signed-in user.</summary>
    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public UserResponseDTO User { get; set; } = new();
    }
}
