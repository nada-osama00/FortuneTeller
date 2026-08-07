namespace FortuneDomain.Shared.Exceptions
{
    public class ForbiddenException(string message = "You do not have access to this resource.")
        : Exception(message)
    {
    }
}
