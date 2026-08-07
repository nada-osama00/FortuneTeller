namespace FortuneApplication.Dtos.Requests.Groups
{
    /// <summary>Recorded from the current user's perspective as the payer (FromUser).</summary>
    public record SettleUpRequest(Guid ToUserId, decimal Amount);
}
