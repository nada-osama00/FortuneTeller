using FortuneDomain.Enums;

namespace FortuneApplication.Dtos.Responses.Groups
{
    public record GroupMemberResponse(Guid UserId, string FullName, string Email, GroupRole Role);
}
