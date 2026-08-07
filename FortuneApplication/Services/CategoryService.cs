using FortuneApplication.Abstractions;
using FortuneApplication.Dtos.Requests.Categories;
using FortuneApplication.Dtos.Responses.Categories;
using FortuneApplication.IServices;
using FortuneApplication.Mappers;
using FortuneDomain.Entities;
using FortuneDomain.Interfaces;
using FortuneDomain.Shared.Exceptions;

namespace FortuneApplication.Services
{
    public class CategoryService(IUnitOfWork unitOfWork, ICurrentUserService currentUser) : ICategoryService
    {
        public async Task<List<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var categories = await unitOfWork.Categories.FindAsync(c => c.UserId == currentUser.UserId, cancellationToken);
            return [.. categories.OrderBy(c => c.Name).Select(c => c.ToResponse())];
        }

        public async Task<CategoryResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            (await GetOwnedAsync(id, cancellationToken)).ToResponse();

        public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
        {
            var name = request.Name.Trim();

            if (await unitOfWork.Categories.AnyAsync(c => c.UserId == currentUser.UserId && c.Name == name, cancellationToken))
            {
                throw new ConflictException($"A category named '{name}' already exists.");
            }

            var category = new Category { UserId = currentUser.UserId, Name = name };

            await unitOfWork.Categories.AddAsync(category, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return category.ToResponse();
        }

        public async Task<CategoryResponse> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
        {
            var category = await GetOwnedAsync(id, cancellationToken);
            var name = request.Name.Trim();

            if (name != category.Name &&
                await unitOfWork.Categories.AnyAsync(c => c.UserId == currentUser.UserId && c.Name == name, cancellationToken))
            {
                throw new ConflictException($"A category named '{name}' already exists.");
            }

            category.Name = name;

            unitOfWork.Categories.Update(category);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return category.ToResponse();
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await GetOwnedAsync(id, cancellationToken);
            unitOfWork.Categories.Remove(category);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<Category> GetOwnedAsync(Guid id, CancellationToken cancellationToken)
        {
            var category = await unitOfWork.Categories.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(Category), id);

            if (category.UserId != currentUser.UserId)
            {
                throw new ForbiddenException();
            }

            return category;
        }
    }
}
