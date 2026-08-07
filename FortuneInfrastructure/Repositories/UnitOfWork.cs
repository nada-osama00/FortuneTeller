using FortuneDomain.Entities;
using FortuneDomain.Interfaces;

namespace FortuneInfrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Users = new Repository<User>(context);
            RefreshTokens = new Repository<RefreshToken>(context);
            Accounts = new Repository<Account>(context);
            Categories = new Repository<Category>(context);
            Transactions = new Repository<Transaction>(context);
            Budgets = new Repository<Budget>(context);
            Groups = new Repository<Group>(context);
            GroupMembers = new Repository<GroupMember>(context);
            GroupExpenses = new Repository<GroupExpense>(context);
            GroupSplits = new Repository<GroupSplit>(context);
            Settlements = new Repository<Settlement>(context);
        }

        public IRepository<User> Users { get; }
        public IRepository<RefreshToken> RefreshTokens { get; }
        public IRepository<Account> Accounts { get; }
        public IRepository<Category> Categories { get; }
        public IRepository<Transaction> Transactions { get; }
        public IRepository<Budget> Budgets { get; }
        public IRepository<Group> Groups { get; }
        public IRepository<GroupMember> GroupMembers { get; }
        public IRepository<GroupExpense> GroupExpenses { get; }
        public IRepository<GroupSplit> GroupSplits { get; }
        public IRepository<Settlement> Settlements { get; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _context.SaveChangesAsync(cancellationToken);
    }
}
