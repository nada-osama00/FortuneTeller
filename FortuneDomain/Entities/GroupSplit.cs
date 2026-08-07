using FortuneDomain.Common;

namespace FortuneDomain.Entities
{
    public class GroupSplit : BaseEntity
    {
        public Guid GroupExpenseId { get; set; }
        public GroupExpense? GroupExpense { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public decimal AmountOwed { get; set; }
    }
}
