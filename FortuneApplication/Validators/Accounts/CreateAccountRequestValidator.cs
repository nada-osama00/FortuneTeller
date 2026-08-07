using FluentValidation;
using FortuneApplication.Dtos.Requests.Accounts;

namespace FortuneApplication.Validators.Accounts
{
    public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
    {
        public CreateAccountRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Type).IsInEnum();
        }
    }
}
