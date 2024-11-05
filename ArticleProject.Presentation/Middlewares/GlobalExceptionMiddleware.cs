using ArticleProject.Application.Common;
using ArticleProject.Domain.Exceptions;
using Serilog;
using System.Net;

namespace ArticleProject.Presentation.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Warning(ex, "Unauthorized access");
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await HandleExceptionAsync(httpContext, ex.Message, HttpStatusCode.Unauthorized);
            } 
            catch (HttpBadForbiddenException ex)
            {
                Log.Warning(ex, "Unauthorized access");
                httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await HandleExceptionAsync(httpContext, ex.Message, HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception occurred");
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await HandleExceptionAsync(httpContext, "An unexpected error occurred", HttpStatusCode.InternalServerError);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, string message, HttpStatusCode statusCode)
        {
            context.Response.ContentType = "application/json";
            var response = Response.Failure(message);
            await context.Response.WriteAsJsonAsync(response);
        }
    }

}
