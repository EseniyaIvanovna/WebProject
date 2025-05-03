using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.ExceptionHandler
{
    public class UnauthorizedExceptionHandler(IProblemDetailsService problemDetailsService)
        : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not UnauthorizedAccessException)
                return false;

            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            httpContext.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Access denied",
                Detail = "You don't have permission to access this resource",
                Instance = httpContext.Request.Path,
                Type = exception.GetType().Name
            };

            var problemDetailsContext = new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
                Exception = exception
            };

            await problemDetailsService.WriteAsync(problemDetailsContext);
            return true;
        }
    }
}