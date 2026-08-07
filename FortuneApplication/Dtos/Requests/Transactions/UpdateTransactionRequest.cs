using FortuneDomain.Enums;

namespace FortuneApplication.Dtos.Requests.Transactions
{
    public record UpdateTransactionRequest(
        Guid AccountId,
        Guid CategoryId,
        DateTime Date,
        decimal Amount,
        string? Description,
        TransactionType Type);
}
