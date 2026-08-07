namespace FortuneApplication.Dtos.Responses.Groups
{
    /// <summary>Positive NetBalance means the group owes this user money; negative means they owe the group.</summary>
    public record BalanceResponse(Guid UserId, string FullName, decimal NetBalance);
}
