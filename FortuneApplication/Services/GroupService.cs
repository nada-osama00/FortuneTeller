using FortuneApplication.Abstractions;
using FortuneApplication.Dtos.Requests.Groups;
using FortuneApplication.Dtos.Responses.Groups;
using FortuneApplication.IServices;
using FortuneApplication.Mappers;
using FortuneDomain.Entities;
using FortuneDomain.Enums;
using FortuneDomain.Interfaces;
using FortuneDomain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FortuneApplication.Services
{
    public class GroupService(IUnitOfWork unitOfWork, ICurrentUserService currentUser) : IGroupService
    {
        public async Task<List<GroupResponse>> GetAllForCurrentUserAsync(CancellationToken cancellationToken = default)
        {
            var groups = await unitOfWork.Groups.Query()
                .Include(g => g.Members).ThenInclude(m => m.User)
                .Where(g => g.Members.Any(m => m.UserId == currentUser.UserId))
                .ToListAsync(cancellationToken);

            return [.. groups.Select(g => g.ToResponse())];
        }

        public async Task<GroupResponse> GetByIdAsync(Guid groupId, CancellationToken cancellationToken = default) =>
            (await GetMemberGroupAsync(groupId, cancellationToken)).ToResponse();

        public async Task<GroupResponse> CreateAsync(CreateGroupRequest request, CancellationToken cancellationToken = default)
        {
            var group = new Group
            {
                Name = request.Name.Trim(),
                CreatedByUserId = currentUser.UserId
            };

            group.Members.Add(new GroupMember { GroupId = group.Id, UserId = currentUser.UserId, Role = GroupRole.Owner });

            await unitOfWork.Groups.AddAsync(group, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(group.Id, cancellationToken);
        }

        public async Task<GroupResponse> AddMemberAsync(Guid groupId, AddGroupMemberRequest request, CancellationToken cancellationToken = default)
        {
            await GetMemberGroupAsync(groupId, cancellationToken);

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await unitOfWork.Users.SingleOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken)
                ?? throw new NotFoundException("User", normalizedEmail);

            if (await unitOfWork.GroupMembers.AnyAsync(m => m.GroupId == groupId && m.UserId == user.Id, cancellationToken))
            {
                throw new ConflictException("This user is already a member of the group.");
            }

            await unitOfWork.GroupMembers.AddAsync(new GroupMember { GroupId = groupId, UserId = user.Id, Role = GroupRole.Member }, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(groupId, cancellationToken);
        }

        public async Task RemoveMemberAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default)
        {
            await GetMemberGroupAsync(groupId, cancellationToken);

            var member = await unitOfWork.GroupMembers.SingleOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId, cancellationToken)
                ?? throw new NotFoundException(nameof(GroupMember), userId);

            unitOfWork.GroupMembers.Remove(member);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<Group> GetMemberGroupAsync(Guid groupId, CancellationToken cancellationToken)
        {
            var group = await unitOfWork.Groups.Query()
                .Include(g => g.Members).ThenInclude(m => m.User)
                .SingleOrDefaultAsync(g => g.Id == groupId, cancellationToken)
                ?? throw new NotFoundException(nameof(Group), groupId);

            if (!group.Members.Any(m => m.UserId == currentUser.UserId))
            {
                throw new ForbiddenException();
            }

            return group;
        }
    }
}
