using FluentValidation;
using FortuneApplication.Dtos.Requests.Budgets;

namespace FortuneApplication.Validators.Budgets
{
    public class SetBudgetRequestValidator : AbstractValidator<SetBudgetRequest>
    {
        public SetBudgetRequestValidator()
        {
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.MonthlyLimit).GreaterThan(0m).WithMessage("Monthly limit must be a positive number.");
        }
    }
}
