using FortuneDomain.Common;

namespace FortuneDomain.Entities
{
    public class Group : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public Guid CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }

        public ICollection<GroupMember> Members { get; set; } = [];
        public ICollection<GroupExpense> Expenses { get; set; } = [];
        public ICollection<Settlement> Settlements { get; set; } = [];
    }
}
