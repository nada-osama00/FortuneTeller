using FortuneApplication.Dtos.Requests.Groups;
using FortuneApplication.Dtos.Responses.Groups;

namespace FortuneApplication.IServices
{
    public interface ISettlementService
    {
        Task<List<BalanceResponse>> GetBalancesAsync(Guid groupId, CancellationToken cancellationToken = default);
        Task<List<SettlementSuggestionResponse>> SimplifyDebtsAsync(Guid groupId, CancellationToken cancellationToken = default);
        Task<SettlementResponse> SettleUpAsync(Guid groupId, SettleUpRequest request, CancellationToken cancellationToken = default);
    }
}
