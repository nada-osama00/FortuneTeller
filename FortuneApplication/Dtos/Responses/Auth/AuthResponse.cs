namespace FortuneApplication.Dtos.Responses.Auth
{
    public record AuthResponse(
        string AccessToken,
        DateTime AccessTokenExpiresAtUtc,
        string RefreshToken,
        DateTime RefreshTokenExpiresAtUtc,
        UserResponse User);
}
