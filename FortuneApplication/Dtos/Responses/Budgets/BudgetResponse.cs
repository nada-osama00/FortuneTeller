namespace FortuneApplication.Dtos.Responses.Budgets
{
    public record BudgetResponse(
        Guid Id,
        Guid CategoryId,
        string CategoryName,
        DateTime Month,
        decimal MonthlyLimit,
        decimal Spent,
        decimal Remaining,
        double ProgressPercentage);
}
