namespace FortuneApplication.Dtos.Responses.Groups
{
    public record GroupResponse(Guid Id, string Name, Guid CreatedByUserId, List<GroupMemberResponse> Members);
}
