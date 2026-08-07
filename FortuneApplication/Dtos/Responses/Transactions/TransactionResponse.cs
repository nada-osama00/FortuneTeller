using FortuneDomain.Enums;

namespace FortuneApplication.Dtos.Responses.Transactions
{
    public record TransactionResponse(
        Guid Id,
        Guid AccountId,
        string AccountName,
        Guid CategoryId,
        string CategoryName,
        DateTime Date,
        decimal Amount,
        string? Description,
        TransactionType Type);
}
