using FortuneApplication.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace FortuneInfrastructure.Services
{
    /// <summary>
    /// Delegates to ASP.NET Core Identity's PBKDF2 implementation rather than rolling custom crypto.
    /// </summary>
    public sealed class PasswordHasher : IPasswordHasher
    {
        private static readonly object DummyUser = new();

        private readonly PasswordHasher<object> _hasher = new();

        public string Hash(string password) => _hasher.HashPassword(DummyUser, password);

        public bool Verify(string password, string passwordHash) =>
            _hasher.VerifyHashedPassword(DummyUser, passwordHash, password)
                is not PasswordVerificationResult.Failed;
    }
}
