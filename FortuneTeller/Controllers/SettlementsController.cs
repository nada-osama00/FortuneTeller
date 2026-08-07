using FluentValidation;
using FortuneApplication.Dtos.Requests.Groups;
using FortuneApplication.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FortuneTeller.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/groups/{groupId:guid}")]
    public class SettlementsController(
        ISettlementService settlementService,
        IValidator<SettleUpRequest> settleUpValidator) : ControllerBase
    {
        [HttpGet("balances")]
        public async Task<IActionResult> GetBalances(Guid groupId, CancellationToken cancellationToken) =>
            Ok(await settlementService.GetBalancesAsync(groupId, cancellationToken));

        [HttpGet("simplify-debts")]
        public async Task<IActionResult> SimplifyDebts(Guid groupId, CancellationToken cancellationToken) =>
            Ok(await settlementService.SimplifyDebtsAsync(groupId, cancellationToken));

        [HttpPost("settlements")]
        public async Task<IActionResult> SettleUp(Guid groupId, SettleUpRequest request, CancellationToken cancellationToken)
        {
            await settleUpValidator.ValidateAndThrowAsync(request, cancellationToken);
            return Ok(await settlementService.SettleUpAsync(groupId, request, cancellationToken));
        }
    }
}
