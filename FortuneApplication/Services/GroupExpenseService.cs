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
    public class GroupExpenseService(IUnitOfWork unitOfWork, ICurrentUserService currentUser) : IGroupExpenseService
    {
        private const decimal Epsilon = 0.01m;

        public async Task<List<GroupExpenseResponse>> GetAllAsync(Guid groupId, CancellationToken cancellationToken = default)
        {
            await EnsureMemberAsync(groupId, cancellationToken);

            var expenses = await unitOfWork.GroupExpenses.Query()
                .Include(e => e.PaidByUser)
                .Include(e => e.Splits).ThenInclude(s => s.User)
                .Where(e => e.GroupId == groupId)
                .OrderByDescending(e => e.Date)
                .ToListAsync(cancellationToken);

            return [.. expenses.Select(e => e.ToResponse())];
        }

        public async Task<GroupExpenseResponse> AddAsync(Guid groupId, AddGroupExpenseRequest request, CancellationToken cancellationToken = default)
        {
            var group = await EnsureMemberAsync(groupId, cancellationToken);

            if (request.Amount <= 0)
            {
                throw new ValidationAppException("Amount must be positive.");
            }

            if (request.Splits.Count == 0)
            {
                throw new ValidationAppException("An expense must be split among at least one member.");
            }

            var memberIds = group.Members.Select(m => m.UserId).ToHashSet();
            if (request.Splits.Any(s => !memberIds.Contains(s.UserId)))
            {
                throw new ValidationAppException("All split participants must be members of the group.");
            }

            var amountsOwed = CalculateSplits(request);

            var expense = new GroupExpense
            {
                GroupId = groupId,
                PaidByUserId = currentUser.UserId,
                Description = request.Description.Trim(),
                Amount = request.Amount,
                SplitType = request.SplitType,
                Date = request.Date
            };

            foreach (var (userId, amountOwed) in amountsOwed)
            {
                expense.Splits.Add(new GroupSplit { GroupExpenseId = expense.Id, UserId = userId, AmountOwed = amountOwed });
            }

            await unitOfWork.GroupExpenses.AddAsync(expense, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var saved = await unitOfWork.GroupExpenses.Query()
                .Include(e => e.PaidByUser)
                .Include(e => e.Splits).ThenInclude(s => s.User)
                .SingleAsync(e => e.Id == expense.Id, cancellationToken);

            return saved.ToResponse();
        }

        /// <summary>
        /// Computes each participant's owed amount for the given split type, validating that the
        /// splits reconcile exactly to the expense total (the domain rule from Section 7.2).
        /// Equal splits use integer-cent division with the remainder assigned to the first participants,
        /// so the sum always matches the total exactly despite rounding.
        /// </summary>
        private static Dictionary<Guid, decimal> CalculateSplits(AddGroupExpenseRequest request)
        {
            var result = new Dictionary<Guid, decimal>();

            switch (request.SplitType)
            {
                case SplitType.Equal:
                    {
                        var count = request.Splits.Count;
                        var totalCents = Math.Round(request.Amount * 100, 0, MidpointRounding.AwayFromZero);
                        var baseCents = (long)totalCents / count;
                        var remainderCents = (long)totalCents % count;

                        for (var i = 0; i < count; i++)
                        {
                            var cents = baseCents + (i < remainderCents ? 1 : 0);
                            result[request.Splits[i].UserId] = cents / 100m;
                        }

                        break;
                    }

                case SplitType.Percentage:
                    {
                        var totalPercentage = request.Splits.Sum(s => s.Value);
                        if (Math.Abs(totalPercentage - 100) > Epsilon)
                        {
                            throw new ValidationAppException("Percentages must add up to 100.");
                        }

                        foreach (var split in request.Splits)
                        {
                            result[split.UserId] = Math.Round(request.Amount * split.Value / 100, 2, MidpointRounding.AwayFromZero);
                        }

                        break;
                    }

                case SplitType.Exact:
                    {
                        var total = request.Splits.Sum(s => s.Value);
                        if (Math.Abs(total - request.Amount) > Epsilon)
                        {
                            throw new ValidationAppException("The sum of all splits must equal the expense's total amount.");
                        }

                        foreach (var split in request.Splits)
                        {
                            result[split.UserId] = split.Value;
                        }

                        break;
                    }

                case SplitType.Shares:
                    {
                        var totalShares = request.Splits.Sum(s => s.Value);
                        if (totalShares <= 0)
                        {
                            throw new ValidationAppException("Total shares must be greater than zero.");
                        }

                        foreach (var split in request.Splits)
                        {
                            result[split.UserId] = Math.Round(request.Amount * split.Value / totalShares, 2, MidpointRounding.AwayFromZero);
                        }

                        break;
                    }

                default:
                    throw new ValidationAppException("Unsupported split type.");
            }

            // Guard against accumulated rounding drift in the Percentage/Shares branches.
            var sum = result.Values.Sum();
            if (Math.Abs(sum - request.Amount) > Epsilon)
            {
                var lastKey = result.Keys.Last();
                result[lastKey] += request.Amount - sum;
            }

            return result;
        }

        private async Task<Group> EnsureMemberAsync(Guid groupId, CancellationToken cancellationToken)
        {
            var group = await unitOfWork.Groups.Query()
                .Include(g => g.Members)
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
