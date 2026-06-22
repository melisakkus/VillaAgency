using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace VillaAgency.Business.Middleware_Reference
{
    /*
     * NOTE FOR REVIEWS / GITHUB:
     * --------------------------------------------------------------------------------
     * This Global Exception Middleware was created for demonstration and future reference purposes.
     * Since this project is a standard ASP.NET Core MVC (WebUI) application, 
     * the built-in 'app.UseExceptionHandler("/Home/Error")' mechanism is used instead 
     * to properly handle web page redirections and maintain correct HTTP status codes.
     * * This class can be safely activated when migrating to a Web API architecture.
     * --------------------------------------------------------------------------------
     */
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // Request is being forwarded to the next component (Controller/Service)
                await _next(httpContext);
            }
            catch(Exception ex)
            {
                // Any exception thrown anywhere in the application will be caught here!
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Default status code is 500 (Internal Server Error)
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "An internal server error occurred. Please try again later.";

            if (exception is FluentValidation.ValidationException valEx)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = "Validation failed for the request.";
            }
            else if(exception is ArgumentNullException || exception is ArgumentException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = exception.Message;
            }

            context.Response.StatusCode = statusCode;

            var errorDetails = new ErrorDetails
            {
                StatusCode = statusCode,
                Message = message,
                Type = exception.GetType().Name
            };

            var jsonResult = JsonSerializer.Serialize(errorDetails);
            return context.Response.WriteAsync(jsonResult);
        }
    }
}
