using FortuneDomain.Enums;

namespace FortuneApplication.Dtos.Requests.Groups
{
    /// <summary>
    /// Value's meaning depends on the expense's SplitType:
    /// Equal -> ignored, Percentage -> percentage of total, Exact -> amount owed, Shares -> share count.
    /// </summary>
    public record GroupSplitInput(Guid UserId, decimal Value);

    public record AddGroupExpenseRequest(
        string Description,
        decimal Amount,
        SplitType SplitType,
        DateTime Date,
        List<GroupSplitInput> Splits);
}
