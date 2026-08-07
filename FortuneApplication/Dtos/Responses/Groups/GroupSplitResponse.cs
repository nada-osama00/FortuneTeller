namespace FortuneApplication.Dtos.Responses.Groups
{
    public record GroupSplitResponse(Guid UserId, string FullName, decimal AmountOwed);
}
