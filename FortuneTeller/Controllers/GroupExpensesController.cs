using FluentValidation;
using FortuneApplication.Dtos.Requests.Groups;
using FortuneApplication.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FortuneTeller.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/groups/{groupId:guid}/expenses")]
    public class GroupExpensesController(
        IGroupExpenseService groupExpenseService,
        IValidator<AddGroupExpenseRequest> addValidator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(Guid groupId, CancellationToken cancellationToken) =>
            Ok(await groupExpenseService.GetAllAsync(groupId, cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Add(Guid groupId, AddGroupExpenseRequest request, CancellationToken cancellationToken)
        {
            await addValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await groupExpenseService.AddAsync(groupId, request, cancellationToken);
            return Ok(result);
        }
    }
}
