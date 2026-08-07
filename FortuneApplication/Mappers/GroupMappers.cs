using FortuneApplication.Dtos.Responses.Groups;
using FortuneDomain.Entities;

namespace FortuneApplication.Mappers
{
    public static class GroupMappers
    {
        public static GroupMemberResponse ToResponse(this GroupMember member) => new(
            member.UserId, member.User?.FullName ?? string.Empty, member.User?.Email ?? string.Empty, member.Role);

        public static GroupResponse ToResponse(this Group group) => new(
            group.Id,
            group.Name,
            group.CreatedByUserId,
            [.. group.Members.Select(m => m.ToResponse())]);

        public static GroupSplitResponse ToResponse(this GroupSplit split) => new(
            split.UserId, split.User?.FullName ?? string.Empty, split.AmountOwed);

        public static GroupExpenseResponse ToResponse(this GroupExpense expense) => new(
            expense.Id,
            expense.GroupId,
            expense.PaidByUserId,
            expense.PaidByUser?.FullName ?? string.Empty,
            expense.Description,
            expense.Amount,
            expense.SplitType,
            expense.Date,
            [.. expense.Splits.Select(s => s.ToResponse())]);

        public static SettlementResponse ToResponse(this Settlement settlement) => new(
            settlement.Id,
            settlement.GroupId,
            settlement.FromUserId,
            settlement.FromUser?.FullName ?? string.Empty,
            settlement.ToUserId,
            settlement.ToUser?.FullName ?? string.Empty,
            settlement.Amount,
            settlement.SettledAt);
    }
}
