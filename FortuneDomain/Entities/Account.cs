using FortuneDomain.Common;
using FortuneDomain.Enums;

namespace FortuneDomain.Entities
{
    public class Account : BaseEntity
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public string Name { get; set; } = string.Empty;
        public AccountType Type { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = [];
    }
}
