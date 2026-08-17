namespace NutriView.API.Configuration
{
    public class JwtSettings
    {
        public const string SectionName = "Jwt";

        public string Issuer { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Symmetric signing key. Must be at least 32 bytes for HMAC-SHA256.
        /// Kept in configuration (user secrets / environment) and never in source.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        public int ExpiryDays { get; set; } = 7;
    }
}
