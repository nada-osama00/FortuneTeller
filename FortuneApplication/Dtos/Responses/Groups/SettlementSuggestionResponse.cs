namespace FortuneApplication.Dtos.Responses.Groups
{
    public record SettlementSuggestionResponse(
        Guid FromUserId,
        string FromUserName,
        Guid ToUserId,
        string ToUserName,
        decimal Amount);
}
