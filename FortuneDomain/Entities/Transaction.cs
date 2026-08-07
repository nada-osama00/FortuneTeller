using FortuneDomain.Common;
using FortuneDomain.Enums;

namespace FortuneDomain.Entities
{
    public class Transaction : BaseEntity
    {
        public Guid AccountId { get; set; }
        public Account? Account { get; set; }

        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public TransactionType Type { get; set; }
    }
}
