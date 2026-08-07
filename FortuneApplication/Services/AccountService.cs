using FortuneApplication.Abstractions;
using FortuneApplication.Dtos.Requests.Accounts;
using FortuneApplication.Dtos.Responses.Accounts;
using FortuneApplication.IServices;
using FortuneApplication.Mappers;
using FortuneDomain.Entities;
using FortuneDomain.Interfaces;
using FortuneDomain.Shared.Exceptions;

namespace FortuneApplication.Services
{
    public class AccountService(IUnitOfWork unitOfWork, ICurrentUserService currentUser) : IAccountService
    {
        public async Task<List<AccountResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var accounts = await unitOfWork.Accounts.FindAsync(a => a.UserId == currentUser.UserId, cancellationToken);
            return [.. accounts.OrderBy(a => a.Name).Select(a => a.ToResponse())];
        }

        public async Task<AccountResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            (await GetOwnedAsync(id, cancellationToken)).ToResponse();

        public async Task<AccountResponse> CreateAsync(CreateAccountRequest request, CancellationToken cancellationToken = default)
        {
            var account = new Account
            {
                UserId = currentUser.UserId,
                Name = request.Name.Trim(),
                Type = request.Type
            };

            await unitOfWork.Accounts.AddAsync(account, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return account.ToResponse();
        }

        public async Task<AccountResponse> UpdateAsync(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken = default)
        {
            var account = await GetOwnedAsync(id, cancellationToken);

            account.Name = request.Name.Trim();
            account.Type = request.Type;

            unitOfWork.Accounts.Update(account);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return account.ToResponse();
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var account = await GetOwnedAsync(id, cancellationToken);
            unitOfWork.Accounts.Remove(account);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<Account> GetOwnedAsync(Guid id, CancellationToken cancellationToken)
        {
            var account = await unitOfWork.Accounts.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(Account), id);

            if (account.UserId != currentUser.UserId)
            {
                throw new ForbiddenException();
            }

            return account;
        }
    }
}
