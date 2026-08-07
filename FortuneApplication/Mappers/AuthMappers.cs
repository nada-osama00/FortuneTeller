using FortuneApplication.Dtos.Responses.Auth;
using FortuneDomain.Entities;

namespace FortuneApplication.Mappers
{
    public static class AuthMappers
    {
        public static UserResponse ToResponse(this User user) => new(user.Id, user.FullName, user.Email);
    }
}
