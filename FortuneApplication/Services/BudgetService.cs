using FortuneApplication.Abstractions;
using FortuneApplication.Dtos.Requests.Budgets;
using FortuneApplication.Dtos.Responses.Budgets;
using FortuneApplication.IServices;
using FortuneDomain.Entities;
using FortuneDomain.Enums;
using FortuneDomain.Interfaces;
using FortuneDomain.Shared.Exceptions;

namespace FortuneApplication.Services
{
    public class BudgetService(IUnitOfWork unitOfWork, ICurrentUserService currentUser) : IBudgetService
    {
        public async Task<List<BudgetResponse>> GetAllAsync(DateTime? month, CancellationToken cancellationToken = default)
        {
            var normalizedMonth = month is { } m ? NormalizeMonth(m) : (DateTime?)null;

            var budgets = await unitOfWork.Budgets.FindAsync(
                b => b.UserId == currentUser.UserId && (normalizedMonth == null || b.Month == normalizedMonth),
                cancellationToken);

            var categories = await unitOfWork.Categories.FindAsync(c => c.UserId == currentUser.UserId, cancellationToken);
            var categoryNames = categories.ToDictionary(c => c.Id, c => c.Name);

            var responses = new List<BudgetResponse>();

            foreach (var budget in budgets)
            {
                var spent = await GetSpentAsync(budget.CategoryId, budget.Month, cancellationToken);
                responses.Add(ToResponse(budget, categoryNames.GetValueOrDefault(budget.CategoryId, string.Empty), spent));
            }

            return [.. responses.OrderByDescending(b => b.Month).ThenBy(b => b.CategoryName)];
        }

        public async Task<BudgetResponse> SetAsync(SetBudgetRequest request, CancellationToken cancellationToken = default)
        {
            if (request.MonthlyLimit <= 0)
            {
                throw new ValidationAppException("Monthly limit must be a positive number.");
            }

            var category = await unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken)
                ?? throw new NotFoundException(nameof(Category), request.CategoryId);

            if (category.UserId != currentUser.UserId)
            {
                throw new ForbiddenException();
            }

            var month = NormalizeMonth(request.Month);

            var budget = await unitOfWork.Budgets.SingleOrDefaultAsync(
                b => b.UserId == currentUser.UserId && b.CategoryId == request.CategoryId && b.Month == month,
                cancellationToken);

            if (budget is null)
            {
                budget = new Budget
                {
                    UserId = currentUser.UserId,
                    CategoryId = request.CategoryId,
                    Month = month,
                    MonthlyLimit = request.MonthlyLimit
                };
                await unitOfWork.Budgets.AddAsync(budget, cancellationToken);
            }
            else
            {
                budget.MonthlyLimit = request.MonthlyLimit;
                unitOfWork.Budgets.Update(budget);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var spent = await GetSpentAsync(budget.CategoryId, budget.Month, cancellationToken);
            return ToResponse(budget, category.Name, spent);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var budget = await unitOfWork.Budgets.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(Budget), id);

            if (budget.UserId != currentUser.UserId)
            {
                throw new ForbiddenException();
            }

            unitOfWork.Budgets.Remove(budget);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<decimal> GetSpentAsync(Guid categoryId, DateTime month, CancellationToken cancellationToken)
        {
            var monthEnd = month.AddMonths(1);

            var transactions = await unitOfWork.Transactions.FindAsync(
                t => t.CategoryId == categoryId
                     && t.Type == TransactionType.Expense
                     && t.Date >= month
                     && t.Date < monthEnd,
                cancellationToken);

            return transactions.Sum(t => t.Amount);
        }

        private static DateTime NormalizeMonth(DateTime month) => new(month.Year, month.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        private static BudgetResponse ToResponse(Budget budget, string categoryName, decimal spent)
        {
            var remaining = budget.MonthlyLimit - spent;
            var progress = budget.MonthlyLimit == 0 ? 0 : (double)(spent / budget.MonthlyLimit) * 100;

            return new BudgetResponse(budget.Id, budget.CategoryId, categoryName, budget.Month, budget.MonthlyLimit, spent, remaining, progress);
        }
    }
}
