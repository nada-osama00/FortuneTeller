using FortuneDomain.Common;

namespace FortuneDomain.Entities
{
    public class Settlement : BaseEntity
    {
        public Guid GroupId { get; set; }
        public Group? Group { get; set; }

        public Guid FromUserId { get; set; }
        public User? FromUser { get; set; }

        public Guid ToUserId { get; set; }
        public User? ToUser { get; set; }

        public decimal Amount { get; set; }
        public DateTime SettledAt { get; set; }
    }
}
