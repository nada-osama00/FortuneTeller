using FortuneDomain.Enums;

namespace FortuneApplication.Dtos.Requests.Transactions
{
    public record TransactionQuery(
        Guid? AccountId = null,
        Guid? CategoryId = null,
        TransactionType? Type = null,
        DateTime? From = null,
        DateTime? To = null,
        int Page = 1,
        int PageSize = 20);
}
