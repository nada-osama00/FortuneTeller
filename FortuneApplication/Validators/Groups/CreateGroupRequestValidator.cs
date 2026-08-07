using FluentValidation;
using FortuneApplication.Dtos.Requests.Groups;

namespace FortuneApplication.Validators.Groups
{
    public class CreateGroupRequestValidator : AbstractValidator<CreateGroupRequest>
    {
        public CreateGroupRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        }
    }
}
