using FluentValidation;
using FortuneApplication.Dtos.Requests.Groups;
using FortuneApplication.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FortuneTeller.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/groups")]
    public class GroupsController(
        IGroupService groupService,
        IValidator<CreateGroupRequest> createValidator,
        IValidator<AddGroupMemberRequest> addMemberValidator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
            Ok(await groupService.GetAllForCurrentUserAsync(cancellationToken));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken) =>
            Ok(await groupService.GetByIdAsync(id, cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Create(CreateGroupRequest request, CancellationToken cancellationToken)
        {
            await createValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await groupService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPost("{id:guid}/members")]
        public async Task<IActionResult> AddMember(Guid id, AddGroupMemberRequest request, CancellationToken cancellationToken)
        {
            await addMemberValidator.ValidateAndThrowAsync(request, cancellationToken);
            return Ok(await groupService.AddMemberAsync(id, request, cancellationToken));
        }

        [HttpDelete("{id:guid}/members/{userId:guid}")]
        public async Task<IActionResult> RemoveMember(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            await groupService.RemoveMemberAsync(id, userId, cancellationToken);
            return NoContent();
        }
    }
}
