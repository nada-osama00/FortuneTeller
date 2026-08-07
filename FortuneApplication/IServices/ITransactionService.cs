using FortuneApplication.Dtos.Requests.Transactions;
using FortuneApplication.Dtos.Responses.Common;
using FortuneApplication.Dtos.Responses.Transactions;

namespace FortuneApplication.IServices
{
    public interface ITransactionService
    {
        Task<PagedResponse<TransactionResponse>> GetAllAsync(TransactionQuery query, CancellationToken cancellationToken = default);
        Task<TransactionResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TransactionResponse> CreateAsync(CreateTransactionRequest request, CancellationToken cancellationToken = default);
        Task<TransactionResponse> UpdateAsync(Guid id, UpdateTransactionRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
