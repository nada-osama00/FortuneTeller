using FortuneDomain.Common;

namespace FortuneDomain.Entities
{
    public class Category : BaseEntity
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<Transaction> Transactions { get; set; } = [];
        public ICollection<Budget> Budgets { get; set; } = [];
    }
}
