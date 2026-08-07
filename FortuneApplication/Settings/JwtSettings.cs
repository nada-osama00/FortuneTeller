using System.ComponentModel.DataAnnotations;

namespace FortuneApplication.Settings
{
    /// <summary>
    /// Bound from the "Jwt" configuration section and validated at startup, so a
    /// misconfigured deployment fails fast instead of issuing broken tokens.
    /// </summary>
    public sealed class JwtSettings
    {
        public const string SectionName = "Jwt";

        [Required]
        public string Issuer { get; init; } = string.Empty;

        [Required]
        public string Audience { get; init; } = string.Empty;

        /// <summary>HMAC-SHA256 signing key. Keep out of source control (user-secrets, env vars, vault).</summary>
        [Required]
        [MinLength(32, ErrorMessage = "The JWT secret must be at least 32 characters (256 bits).")]
        public string Secret { get; init; } = string.Empty;

        [Range(1, 24 * 60)]
        public int AccessTokenLifetimeMinutes { get; init; } = 15;

        [Range(1, 365)]
        public int RefreshTokenLifetimeDays { get; init; } = 7;
    }
}
