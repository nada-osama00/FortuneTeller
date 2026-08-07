using FortuneApplication.Dtos.Responses.Transactions;
using FortuneDomain.Entities;

namespace FortuneApplication.Mappers
{
    public static class TransactionMappers
    {
        public static TransactionResponse ToResponse(this Transaction transaction) => new(
            transaction.Id,
            transaction.AccountId,
            transaction.Account?.Name ?? string.Empty,
            transaction.CategoryId,
            transaction.Category?.Name ?? string.Empty,
            transaction.Date,
            transaction.Amount,
            transaction.Description,
            transaction.Type);
    }
}
