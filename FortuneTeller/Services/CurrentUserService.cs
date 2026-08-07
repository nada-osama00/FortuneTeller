using System.Security.Claims;
using FortuneApplication.Abstractions;
using FortuneDomain.Shared.Exceptions;
using Microsoft.IdentityModel.JsonWebTokens;

namespace FortuneTeller.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public Guid UserId
        {
            get
            {
                var subject = httpContextAccessor.HttpContext?.User
                    .FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (subject is null || !Guid.TryParse(subject, out var userId))
                {
                    throw new UnauthorizedAppException("No authenticated user on the current request.");
                }

                return userId;
            }
        }
    }
}
