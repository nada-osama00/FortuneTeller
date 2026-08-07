namespace FortuneDomain.Shared.Exceptions
{
    /// <summary>
    /// Thrown for business-rule violations that map to a 400 response
    /// (as opposed to FluentValidation's input-shape errors, which the pipeline handles separately).
    /// </summary>
    public class ValidationAppException(string message) : Exception(message)
    {
    }
}
