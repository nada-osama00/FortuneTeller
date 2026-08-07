using FluentValidation;
using FortuneApplication.Dtos.Requests.Groups;

namespace FortuneApplication.Validators.Groups
{
    public class AddGroupMemberRequestValidator : AbstractValidator<AddGroupMemberRequest>
    {
        public AddGroupMemberRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}
