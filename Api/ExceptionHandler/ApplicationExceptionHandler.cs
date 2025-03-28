﻿using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.ExceptionHandler
{
    public class ApplicationExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not BaseApplicationException e)
                return false;

            httpContext.Response.StatusCode = (int)e.StatusCode;
            httpContext.Response.ContentType = "application/problen+json";

            var problenDetails = new ProblemDetails
            {
                Status = (int)e.StatusCode,
                Title = e.Title,
                Detail = e.Message,
                Instance = httpContext.Request.Path,
                Type = e.GetType().Name
            };

            var problenDetailsContext = new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problenDetails,
                Exception = e
            };
            await problemDetailsService.WriteAsync(problenDetailsContext);
            return true;
        }
    }
}
