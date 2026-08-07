namespace FortuneApplication.Dtos.Responses.Groups
{
    public record SettlementResponse(
        Guid Id,
        Guid GroupId,
        Guid FromUserId,
        string FromUserName,
        Guid ToUserId,
        string ToUserName,
        decimal Amount,
        DateTime SettledAt);
}
