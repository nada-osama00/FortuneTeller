using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FortuneApplication.Abstractions;
using FortuneApplication.Settings;
using FortuneDomain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FortuneInfrastructure.Services
{
    public sealed class JwtTokenGenerator(IOptions<JwtSettings> jwtOptions) : IJwtTokenGenerator
    {
        private readonly JwtSettings _settings = jwtOptions.Value;

        public AccessTokenResult GenerateAccessToken(User user)
        {
            var utcNow = DateTime.UtcNow;
            var expiresAtUtc = utcNow.AddMinutes(_settings.AccessTokenLifetimeMinutes);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                NotBefore = utcNow,
                Expires = expiresAtUtc,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret)),
                    SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                ])
            };

            var token = new JsonWebTokenHandler().CreateToken(descriptor);

            return new AccessTokenResult(token, expiresAtUtc);
        }

        public RefreshTokenResult GenerateRefreshToken()
        {
            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var expiresAtUtc = DateTime.UtcNow.AddDays(_settings.RefreshTokenLifetimeDays);

            return new RefreshTokenResult(rawToken, HashToken(rawToken), expiresAtUtc);
        }

        public string HashToken(string token) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
