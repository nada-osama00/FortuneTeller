using FortuneApplication.Abstractions;
using FortuneApplication.Dtos.Requests.Groups;
using FortuneApplication.Dtos.Responses.Groups;
using FortuneApplication.IServices;
using FortuneApplication.Mappers;
using FortuneDomain.Entities;
using FortuneDomain.Interfaces;
using FortuneDomain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FortuneApplication.Services
{
    public class SettlementService(IUnitOfWork unitOfWork, ICurrentUserService currentUser) : ISettlementService
    {
        public async Task<List<BalanceResponse>> GetBalancesAsync(Guid groupId, CancellationToken cancellationToken = default)
        {
            var group = await EnsureMemberAsync(groupId, cancellationToken);
            var balances = await CalculateNetBalancesAsync(groupId, cancellationToken);

            return [.. group.Members
                .Select(m => new BalanceResponse(m.UserId, m.User?.FullName ?? string.Empty, balances.GetValueOrDefault(m.UserId, 0)))
                .OrderByDescending(b => b.NetBalance)];
        }

        public async Task<List<SettlementSuggestionResponse>> SimplifyDebtsAsync(Guid groupId, CancellationToken cancellationToken = default)
        {
            var group = await EnsureMemberAsync(groupId, cancellationToken);
            var balances = await CalculateNetBalancesAsync(groupId, cancellationToken);
            var names = group.Members.ToDictionary(m => m.UserId, m => m.User?.FullName ?? string.Empty);

            // Greedy net-balance simplification: repeatedly settle the largest creditor against the largest debtor.
            var creditors = new List<(Guid UserId, decimal Amount)>();
            var debtors = new List<(Guid UserId, decimal Amount)>();

            foreach (var (userId, balance) in balances)
            {
                if (balance > 0.01m)
                {
                    creditors.Add((userId, balance));
                }
                else if (balance < -0.01m)
                {
                    debtors.Add((userId, -balance));
                }
            }

            creditors.Sort((a, b) => b.Amount.CompareTo(a.Amount));
            debtors.Sort((a, b) => b.Amount.CompareTo(a.Amount));

            var suggestions = new List<SettlementSuggestionResponse>();
            var ci = 0;
            var di = 0;

            while (ci < creditors.Count && di < debtors.Count)
            {
                var (creditorId, creditAmount) = creditors[ci];
                var (debtorId, debtAmount) = debtors[di];

                var settleAmount = Math.Min(creditAmount, debtAmount);
                suggestions.Add(new SettlementSuggestionResponse(
                    debtorId, names.GetValueOrDefault(debtorId, string.Empty),
                    creditorId, names.GetValueOrDefault(creditorId, string.Empty),
                    Math.Round(settleAmount, 2)));

                creditAmount -= settleAmount;
                debtAmount -= settleAmount;

                if (creditAmount <= 0.01m) ci++; else creditors[ci] = (creditorId, creditAmount);
                if (debtAmount <= 0.01m) di++; else debtors[di] = (debtorId, debtAmount);
            }

            return suggestions;
        }

        public async Task<SettlementResponse> SettleUpAsync(Guid groupId, SettleUpRequest request, CancellationToken cancellationToken = default)
        {
            var group = await EnsureMemberAsync(groupId, cancellationToken);

            if (request.Amount <= 0)
            {
                throw new ValidationAppException("Amount must be positive.");
            }

            if (request.ToUserId == currentUser.UserId)
            {
                throw new ValidationAppException("fromUser and toUser must be different people.");
            }

            if (!group.Members.Any(m => m.UserId == request.ToUserId))
            {
                throw new NotFoundException("GroupMember", request.ToUserId);
            }

            var settlement = new Settlement
            {
                GroupId = groupId,
                FromUserId = currentUser.UserId,
                ToUserId = request.ToUserId,
                Amount = request.Amount,
                SettledAt = DateTime.UtcNow
            };

            await unitOfWork.Settlements.AddAsync(settlement, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var saved = await unitOfWork.Settlements.Query()
                .Include(s => s.FromUser)
                .Include(s => s.ToUser)
                .SingleAsync(s => s.Id == settlement.Id, cancellationToken);

            return saved.ToResponse();
        }

        private async Task<Dictionary<Guid, decimal>> CalculateNetBalancesAsync(Guid groupId, CancellationToken cancellationToken)
        {
            var expenses = await unitOfWork.GroupExpenses.Query()
                .Include(e => e.Splits)
                .Where(e => e.GroupId == groupId)
                .ToListAsync(cancellationToken);

            var settlements = await unitOfWork.Settlements.FindAsync(s => s.GroupId == groupId, cancellationToken);

            var balances = new Dictionary<Guid, decimal>();

            void Add(Guid userId, decimal amount) =>
                balances[userId] = balances.GetValueOrDefault(userId, 0) + amount;

            foreach (var expense in expenses)
            {
                Add(expense.PaidByUserId, expense.Amount);
                foreach (var split in expense.Splits)
                {
                    Add(split.UserId, -split.AmountOwed);
                }
            }

            foreach (var settlement in settlements)
            {
                // Paying off a debt moves the payer's balance toward zero and the receiver's balance toward zero.
                Add(settlement.FromUserId, settlement.Amount);
                Add(settlement.ToUserId, -settlement.Amount);
            }

            return balances;
        }

        private async Task<Group> EnsureMemberAsync(Guid groupId, CancellationToken cancellationToken)
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
