using FortuneApplication.Dtos.Requests.Groups;
using FortuneApplication.Dtos.Responses.Groups;

namespace FortuneApplication.IServices
{
    public interface IGroupExpenseService
    {
        Task<List<GroupExpenseResponse>> GetAllAsync(Guid groupId, CancellationToken cancellationToken = default);
        Task<GroupExpenseResponse> AddAsync(Guid groupId, AddGroupExpenseRequest request, CancellationToken cancellationToken = default);
    }
}
