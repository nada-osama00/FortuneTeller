using FortuneApplication.Dtos.Requests.Auth;
using FortuneApplication.Dtos.Responses.Auth;

namespace FortuneApplication.IServices
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
        Task LogoutAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    }
}
