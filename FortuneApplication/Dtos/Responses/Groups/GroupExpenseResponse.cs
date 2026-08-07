using FortuneDomain.Enums;

namespace FortuneApplication.Dtos.Responses.Groups
{
    public record GroupExpenseResponse(
        Guid Id,
        Guid GroupId,
        Guid PaidByUserId,
        string PaidByName,
        string Description,
        decimal Amount,
        SplitType SplitType,
        DateTime Date,
        List<GroupSplitResponse> Splits);
}
