using FortuneDomain.Common;
using FortuneDomain.Enums;

namespace FortuneDomain.Entities
{
    public class GroupMember : BaseEntity
    {
        public Guid GroupId { get; set; }
        public Group? Group { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public GroupRole Role { get; set; }
    }
}
