using FortuneApplication.Dtos.Requests.Budgets;
using FortuneApplication.Dtos.Responses.Budgets;

namespace FortuneApplication.IServices
{
    public interface IBudgetService
    {
        Task<List<BudgetResponse>> GetAllAsync(DateTime? month, CancellationToken cancellationToken = default);
        Task<BudgetResponse> SetAsync(SetBudgetRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
