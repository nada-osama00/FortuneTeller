using FluentValidation;
using FortuneApplication.Dtos.Requests.Auth;

namespace FortuneApplication.Validators.Auth
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long.");
        }
    }
}
