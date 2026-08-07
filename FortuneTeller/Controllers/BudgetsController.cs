using FluentValidation;
using FortuneApplication.Dtos.Requests.Budgets;
using FortuneApplication.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FortuneTeller.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/budgets")]
    public class BudgetsController(IBudgetService budgetService, IValidator<SetBudgetRequest> setValidator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DateTime? month, CancellationToken cancellationToken) =>
            Ok(await budgetService.GetAllAsync(month, cancellationToken));

        [HttpPut]
        public async Task<IActionResult> Set(SetBudgetRequest request, CancellationToken cancellationToken)
        {
            await setValidator.ValidateAndThrowAsync(request, cancellationToken);
            return Ok(await budgetService.SetAsync(request, cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await budgetService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
