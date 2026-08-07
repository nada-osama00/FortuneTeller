using FluentValidation;
using FortuneApplication.Dtos.Requests.Groups;

namespace FortuneApplication.Validators.Groups
{
    public class AddGroupExpenseRequestValidator : AbstractValidator<AddGroupExpenseRequest>
    {
        public AddGroupExpenseRequestValidator()
        {
            RuleFor(x => x.Description).NotEmpty().MaximumLength(300);
            RuleFor(x => x.Amount).GreaterThan(0m);
            RuleFor(x => x.SplitType).IsInEnum();
            RuleFor(x => x.Date).LessThanOrEqualTo(_ => DateTime.UtcNow).WithMessage("Date cannot be in the future.");
            RuleFor(x => x.Splits).NotEmpty().WithMessage("An expense must be split among at least one member.");
            RuleForEach(x => x.Splits).ChildRules(split =>
            {
                split.RuleFor(s => s.UserId).NotEmpty();
            });
        }
    }
}
