using FluentValidation;
using FortuneApplication.Dtos.Requests.Auth;
using FortuneApplication.IServices;
using Microsoft.AspNetCore.Mvc;

namespace FortuneTeller.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(
        IAuthService authService,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator,
        IValidator<RefreshTokenRequest> refreshTokenValidator) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
        {
            await registerValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await authService.RegisterAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            await loginValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await authService.LoginAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            await refreshTokenValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await authService.RefreshAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            await refreshTokenValidator.ValidateAndThrowAsync(request, cancellationToken);
            await authService.LogoutAsync(request, cancellationToken);
            return NoContent();
        }
    }
}
