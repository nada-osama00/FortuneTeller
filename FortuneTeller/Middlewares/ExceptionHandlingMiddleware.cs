using System.Net;
using System.Text.Json;
using FortuneDomain.Shared.Exceptions;

namespace FortuneTeller.Middlewares
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var (statusCode, title) = ex switch
                {
                    NotFoundException => (HttpStatusCode.NotFound, "Not Found"),
                    ForbiddenException => (HttpStatusCode.Forbidden, "Forbidden"),
                    UnauthorizedAppException => (HttpStatusCode.Unauthorized, "Unauthorized"),
                    ConflictException => (HttpStatusCode.Conflict, "Conflict"),
                    ValidationAppException => (HttpStatusCode.BadRequest, "Bad Request"),
                    FluentValidation.ValidationException => (HttpStatusCode.BadRequest, "Validation Failed"),
                    _ => (HttpStatusCode.InternalServerError, "Internal Server Error")
                };

                if (statusCode == HttpStatusCode.InternalServerError)
                {
                    logger.LogError(ex, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);
                }

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = (int)statusCode;

                var problem = new
                {
                    title,
                    status = (int)statusCode,
                    detail = ex.Message,
                    errors = ex is FluentValidation.ValidationException validationException
                        ? validationException.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                        : null
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            }
        }
    }
}
