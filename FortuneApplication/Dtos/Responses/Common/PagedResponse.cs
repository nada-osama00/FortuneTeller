namespace FortuneApplication.Dtos.Responses.Common
{
    public record PagedResponse<T>(List<T> Items, int TotalCount, int Page, int PageSize);
}
