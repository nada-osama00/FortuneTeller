using FluentValidation;
using FortuneApplication.Dtos.Requests.Transactions;

namespace FortuneApplication.Validators.Transactions
{
    public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
    {
        public CreateTransactionRequestValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.Amount).NotEqual(0m).WithMessage("Amount cannot be zero.");
            RuleFor(x => x.Date).LessThanOrEqualTo(_ => DateTime.UtcNow).WithMessage("Date cannot be in the future.");
            RuleFor(x => x.Description).MaximumLength(500);
            RuleFor(x => x.Type).IsInEnum();
        }
    }
}
