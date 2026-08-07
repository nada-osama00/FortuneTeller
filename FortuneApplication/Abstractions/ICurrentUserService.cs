namespace FortuneApplication.Abstractions
{
    /// <summary>
    /// Exposes the authenticated user for the current request.
    /// Implemented in the API layer (reads the JWT claims off HttpContext)
    /// so the Application layer never has to know ASP.NET Core exists.
    /// </summary>
    public interface ICurrentUserService
    {
        Guid UserId { get; }
    }
}
