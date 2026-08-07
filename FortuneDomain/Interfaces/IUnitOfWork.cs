using FortuneDomain.Entities;

namespace FortuneDomain.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<User> Users { get; }
        IRepository<RefreshToken> RefreshTokens { get; }
        IRepository<Account> Accounts { get; }
        IRepository<Category> Categories { get; }
        IRepository<Transaction> Transactions { get; }
        IRepository<Budget> Budgets { get; }
        IRepository<Group> Groups { get; }
        IRepository<GroupMember> GroupMembers { get; }
        IRepository<GroupExpense> GroupExpenses { get; }
        IRepository<GroupSplit> GroupSplits { get; }
        IRepository<Settlement> Settlements { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
