namespace FortuneApplication.Dtos.Requests.Budgets
{
    /// <summary>Month is normalized server-side to the first day of the given month.</summary>
    public record SetBudgetRequest(Guid CategoryId, DateTime Month, decimal MonthlyLimit);
}
