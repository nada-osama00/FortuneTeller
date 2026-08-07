using FortuneApplication.Dtos.Requests.Accounts;
using FortuneApplication.Dtos.Responses.Accounts;

namespace FortuneApplication.IServices
{
    public interface IAccountService
    {
        Task<List<AccountResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<AccountResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<AccountResponse> CreateAsync(CreateAccountRequest request, CancellationToken cancellationToken = default);
        Task<AccountResponse> UpdateAsync(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
