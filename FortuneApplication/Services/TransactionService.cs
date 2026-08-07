using FortuneApplication.Abstractions;
using FortuneApplication.Dtos.Requests.Transactions;
using FortuneApplication.Dtos.Responses.Common;
using FortuneApplication.Dtos.Responses.Transactions;
using FortuneApplication.IServices;
using FortuneApplication.Mappers;
using FortuneDomain.Entities;
using FortuneDomain.Interfaces;
using FortuneDomain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FortuneApplication.Services
{
    public class TransactionService(IUnitOfWork unitOfWork, ICurrentUserService currentUser) : ITransactionService
    {
        public async Task<PagedResponse<TransactionResponse>> GetAllAsync(TransactionQuery query, CancellationToken cancellationToken = default)
        {
            var transactions = unitOfWork.Transactions.Query()
                .Include(t => t.Account)
                .Include(t => t.Category)
                .Where(t => t.Account!.UserId == currentUser.UserId);

            if (query.AccountId is { } accountId)
            {
                transactions = transactions.Where(t => t.AccountId == accountId);
            }

            if (query.CategoryId is { } categoryId)
            {
                transactions = transactions.Where(t => t.CategoryId == categoryId);
            }

            if (query.Type is { } type)
            {
                transactions = transactions.Where(t => t.Type == type);
            }

            if (query.From is { } from)
            {
                transactions = transactions.Where(t => t.Date >= from);
            }

            if (query.To is { } to)
            {
                transactions = transactions.Where(t => t.Date <= to);
            }

            var totalCount = await transactions.CountAsync(cancellationToken);

            var page = Math.Max(query.Page, 1);
            var pageSize = Math.Clamp(query.PageSize, 1, 100);

            var items = await transactions
                .OrderByDescending(t => t.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => t.ToResponse())
                .ToListAsync(cancellationToken);

            return new PagedResponse<TransactionResponse>(items, totalCount, page, pageSize);
        }

        public async Task<TransactionResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            (await GetOwnedAsync(id, cancellationToken)).ToResponse();

        public async Task<TransactionResponse> CreateAsync(CreateTransactionRequest request, CancellationToken cancellationToken = default)
        {
            await EnsureOwnedAccountAndCategoryAsync(request.AccountId, request.CategoryId, cancellationToken);
            EnsureValid(request.Amount, request.Date);

            var transaction = new Transaction
            {
                AccountId = request.AccountId,
                CategoryId = request.CategoryId,
                Date = request.Date,
                Amount = request.Amount,
                Description = request.Description?.Trim(),
                Type = request.Type
            };

            await unitOfWork.Transactions.AddAsync(transaction, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(transaction.Id, cancellationToken);
        }

        public async Task<TransactionResponse> UpdateAsync(Guid id, UpdateTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var transaction = await GetOwnedAsync(id, cancellationToken);
            await EnsureOwnedAccountAndCategoryAsync(request.AccountId, request.CategoryId, cancellationToken);
            EnsureValid(request.Amount, request.Date);

            transaction.AccountId = request.AccountId;
            transaction.CategoryId = request.CategoryId;
            transaction.Date = request.Date;
            transaction.Amount = request.Amount;
            transaction.Description = request.Description?.Trim();
            transaction.Type = request.Type;

            unitOfWork.Transactions.Update(transaction);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(transaction.Id, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var transaction = await GetOwnedAsync(id, cancellationToken);
            unitOfWork.Transactions.Remove(transaction);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static void EnsureValid(decimal amount, DateTime date)
        {
            if (amount == 0)
            {
                throw new ValidationAppException("Amount cannot be zero.");
            }

            if (date.Date > DateTime.UtcNow.Date)
            {
                throw new ValidationAppException("Date cannot be in the future.");
            }
        }

        private async Task EnsureOwnedAccountAndCategoryAsync(Guid accountId, Guid categoryId, CancellationToken cancellationToken)
        {
            var account = await unitOfWork.Accounts.GetByIdAsync(accountId, cancellationToken)
                ?? throw new NotFoundException(nameof(Account), accountId);

            if (account.UserId != currentUser.UserId)
            {
                throw new ForbiddenException();
            }

            var category = await unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken)
                ?? throw new NotFoundException(nameof(Category), categoryId);

            if (category.UserId != currentUser.UserId)
            {
                throw new ForbiddenException();
            }
        }

        private async Task<Transaction> GetOwnedAsync(Guid id, CancellationToken cancellationToken)
        {
            var transaction = await unitOfWork.Transactions.Query()
                .Include(t => t.Account)
                .Include(t => t.Category)
                .SingleOrDefaultAsync(t => t.Id == id, cancellationToken)
                ?? throw new NotFoundException(nameof(Transaction), id);

            if (transaction.Account?.UserId != currentUser.UserId)
            {
                throw new ForbiddenException();
            }

            return transaction;
        }
    }
}
