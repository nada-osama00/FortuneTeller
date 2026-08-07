using FortuneDomain.Common;

namespace FortuneDomain.Entities
{
    public class Budget : BaseEntity
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        /// <summary>First day of the budgeted month, e.g. 2026-08-01. Time component is always midnight.</summary>
        public DateTime Month { get; set; }
        public decimal MonthlyLimit { get; set; }
    }
}
