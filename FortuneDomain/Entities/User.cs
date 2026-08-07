using FortuneDomain.Common;

namespace FortuneDomain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<Account> Accounts { get; set; } = [];
        public ICollection<Category> Categories { get; set; } = [];
        public ICollection<Budget> Budgets { get; set; } = [];
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
        public ICollection<GroupMember> GroupMemberships { get; set; } = [];
    }
}
