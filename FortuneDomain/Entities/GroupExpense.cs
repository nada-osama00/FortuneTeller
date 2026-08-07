using FortuneDomain.Common;
using FortuneDomain.Enums;

namespace FortuneDomain.Entities
{
    public class GroupExpense : BaseEntity
    {
        public Guid GroupId { get; set; }
        public Group? Group { get; set; }

        public Guid PaidByUserId { get; set; }
        public User? PaidByUser { get; set; }

        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public SplitType SplitType { get; set; }
        public DateTime Date { get; set; }

        public ICollection<GroupSplit> Splits { get; set; } = [];
    }
}
