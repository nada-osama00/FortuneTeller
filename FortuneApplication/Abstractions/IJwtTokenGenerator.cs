using FortuneDomain.Entities;

namespace FortuneApplication.Abstractions
{
    public record AccessTokenResult(string Token, DateTime ExpiresAtUtc);
    public record RefreshTokenResult(string RawToken, string HashedToken, DateTime ExpiresAtUtc);

    public interface IJwtTokenGenerator
    {
        AccessTokenResult GenerateAccessToken(User user);
        RefreshTokenResult GenerateRefreshToken();
        string HashToken(string token);
    }
}
