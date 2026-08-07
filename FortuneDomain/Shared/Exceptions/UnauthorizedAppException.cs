namespace FortuneDomain.Shared.Exceptions
{
    public class UnauthorizedAppException(string message = "Invalid credentials.") : Exception(message)
    {
    }
}
