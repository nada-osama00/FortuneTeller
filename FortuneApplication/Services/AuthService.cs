using FortuneApplication.Abstractions;
using FortuneApplication.Dtos.Requests.Auth;
using FortuneApplication.Dtos.Responses.Auth;
using FortuneApplication.IServices;
using FortuneApplication.Mappers;
using FortuneDomain.Entities;
using FortuneDomain.Interfaces;
using FortuneDomain.Shared.Exceptions;

namespace FortuneApplication.Services
{
    public class AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator) : IAuthService
    {
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            if (await unitOfWork.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken))
            {
                throw new ConflictException("An account with this email already exists.");
            }

            var user = new User
            {
                FullName = request.FullName.Trim(),
                Email = normalizedEmail,
                PasswordHash = passwordHasher.Hash(request.Password)
            };

            await unitOfWork.Users.AddAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return await IssueTokensAsync(user, cancellationToken);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await unitOfWork.Users.SingleOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken)
                ?? throw new UnauthorizedAppException();

            if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAppException();
            }

            return await IssueTokensAsync(user, cancellationToken);
        }

        public async Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var hashedToken = jwtTokenGenerator.HashToken(request.RefreshToken);
            var storedToken = await unitOfWork.RefreshTokens.SingleOrDefaultAsync(rt => rt.TokenHash == hashedToken, cancellationToken)
                ?? throw new UnauthorizedAppException("Invalid or expired refresh token.");

            if (!storedToken.IsActive)
            {
                throw new UnauthorizedAppException("Invalid or expired refresh token.");
            }

            var user = await unitOfWork.Users.GetByIdAsync(storedToken.UserId, cancellationToken)
                ?? throw new UnauthorizedAppException();

            // Rotate: revoke the old token and issue a new pair.
            storedToken.RevokedAt = DateTime.UtcNow;
            unitOfWork.RefreshTokens.Update(storedToken);

            return await IssueTokensAsync(user, cancellationToken);
        }

        public async Task LogoutAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var hashedToken = jwtTokenGenerator.HashToken(request.RefreshToken);
            var storedToken = await unitOfWork.RefreshTokens.SingleOrDefaultAsync(rt => rt.TokenHash == hashedToken, cancellationToken);

            if (storedToken is null || !storedToken.IsActive)
            {
                return;
            }

            storedToken.RevokedAt = DateTime.UtcNow;
            unitOfWork.RefreshTokens.Update(storedToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<AuthResponse> IssueTokensAsync(User user, CancellationToken cancellationToken)
        {
            var accessToken = jwtTokenGenerator.GenerateAccessToken(user);
            var refreshToken = jwtTokenGenerator.GenerateRefreshToken();

            await unitOfWork.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshToken.HashedToken,
                ExpiresAt = refreshToken.ExpiresAtUtc
            }, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthResponse(
                accessToken.Token,
                accessToken.ExpiresAtUtc,
                refreshToken.RawToken,
                refreshToken.ExpiresAtUtc,
                user.ToResponse());
        }
    }
}
