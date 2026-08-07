using FluentValidation;
using FortuneApplication.Dtos.Requests.Groups;

namespace FortuneApplication.Validators.Groups
{
    public class SettleUpRequestValidator : AbstractValidator<SettleUpRequest>
    {
        public SettleUpRequestValidator()
        {
            RuleFor(x => x.ToUserId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0m);
        }
    }
}
