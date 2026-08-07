using FluentValidation;
using FortuneApplication.Dtos.Requests.Transactions;
using FortuneApplication.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FortuneTeller.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/transactions")]
    public class TransactionsController(
        ITransactionService transactionService,
        IValidator<CreateTransactionRequest> createValidator,
        IValidator<UpdateTransactionRequest> updateValidator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TransactionQuery query, CancellationToken cancellationToken) =>
            Ok(await transactionService.GetAllAsync(query, cancellationToken));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken) =>
            Ok(await transactionService.GetByIdAsync(id, cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionRequest request, CancellationToken cancellationToken)
        {
            await createValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await transactionService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateTransactionRequest request, CancellationToken cancellationToken)
        {
            await updateValidator.ValidateAndThrowAsync(request, cancellationToken);
            return Ok(await transactionService.UpdateAsync(id, request, cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await transactionService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
