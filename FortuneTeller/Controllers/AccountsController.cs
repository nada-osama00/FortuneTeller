using FluentValidation;
using FortuneApplication.Dtos.Requests.Accounts;
using FortuneApplication.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FortuneTeller.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/accounts")]
    public class AccountsController(
        IAccountService accountService,
        IValidator<CreateAccountRequest> createValidator,
        IValidator<UpdateAccountRequest> updateValidator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
            Ok(await accountService.GetAllAsync(cancellationToken));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken) =>
            Ok(await accountService.GetByIdAsync(id, cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountRequest request, CancellationToken cancellationToken)
        {
            await createValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await accountService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken)
        {
            await updateValidator.ValidateAndThrowAsync(request, cancellationToken);
            return Ok(await accountService.UpdateAsync(id, request, cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await accountService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
