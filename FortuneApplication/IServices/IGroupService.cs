using FortuneApplication.Dtos.Requests.Groups;
using FortuneApplication.Dtos.Responses.Groups;

namespace FortuneApplication.IServices
{
    public interface IGroupService
    {
        Task<List<GroupResponse>> GetAllForCurrentUserAsync(CancellationToken cancellationToken = default);
        Task<GroupResponse> GetByIdAsync(Guid groupId, CancellationToken cancellationToken = default);
        Task<GroupResponse> CreateAsync(CreateGroupRequest request, CancellationToken cancellationToken = default);
        Task<GroupResponse> AddMemberAsync(Guid groupId, AddGroupMemberRequest request, CancellationToken cancellationToken = default);
        Task RemoveMemberAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default);
    }
}
