using FortuneApplication.Dtos.Requests.Categories;
using FortuneApplication.Dtos.Responses.Categories;

namespace FortuneApplication.IServices
{
    public interface ICategoryService
    {
        Task<List<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<CategoryResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
        Task<CategoryResponse> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
