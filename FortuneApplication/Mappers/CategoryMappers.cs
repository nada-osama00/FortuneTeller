using FortuneApplication.Dtos.Responses.Categories;
using FortuneDomain.Entities;

namespace FortuneApplication.Mappers
{
    public static class CategoryMappers
    {
        public static CategoryResponse ToResponse(this Category category) => new(category.Id, category.Name);
    }
}
