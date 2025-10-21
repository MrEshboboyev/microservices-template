using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Exceptions;
using SharedLibrary.Services;
using System.Net;
using System.Text.Json;

namespace SharedLibrary.Middleware;

public class GlobalException(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Declare default variables
        string message = "Sorry, internal server error occurred. Kindly try again";
        int statusCode = (int)HttpStatusCode.InternalServerError;
        string title = "Error";
        string errorCode = "INTERNAL_ERROR";
        
        // Get logging service
        var loggingService = context.RequestServices.GetService<ILoggingService>();
        
        try
        {
            await next(context);
            // check if Response is Too Many Request // 429 Status Code
            if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
                title = "Warning";
                message = "Too many request made.";
                errorCode = "TOO_MANY_REQUESTS";
                statusCode = StatusCodes.Status429TooManyRequests;
                loggingService?.LogWarning("Too many requests made. Status code: {StatusCode}", statusCode);
                await ModifyHeader(context, title, message, statusCode, errorCode);
            }
            // If Response is UnAuthorized // 401 status code
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                title = "Alert";
                message = "You are not authorized to access.";
                errorCode = "UNAUTHORIZED";
                statusCode = StatusCodes.Status401Unauthorized;
                loggingService?.LogWarning("Unauthorized access attempt. Status code: {StatusCode}", statusCode);
                await ModifyHeader(context, title, message, statusCode, errorCode);
            }

            // If Response is Forbidden // 403 status code
            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                title = "Out of Access";
                message = "You are not allowed/required to access.";
                errorCode = "FORBIDDEN";
                statusCode = StatusCodes.Status403Forbidden;
                loggingService?.LogWarning("Forbidden access attempt. Status code: {StatusCode}", statusCode);
                await ModifyHeader(context, title, message, statusCode, errorCode);
            }
        }
        catch (BaseCustomException ex)
        {
            // Handle custom exceptions
            loggingService?.LogError(ex, "Custom exception occurred: {ErrorCode}", ex.ErrorCode);
            await ModifyHeader(context, ex.Title, ex.Message, ex.StatusCode, ex.ErrorCode);
        }
        catch (Exception ex)
        {
            // Log original Exceptions / File, Debugger, Console
            loggingService?.LogError(ex, "Unhandled exception occurred");
            
            // check if Exception is TimeOut / 408 Request Timeout
            if (ex is TaskCanceledException || ex is TimeoutException)
            {
                title = "Out of time";
                message = "Request timeout ... Try again!";
                errorCode = "REQUEST_TIMEOUT";
                statusCode = StatusCodes.Status408RequestTimeout;
                loggingService?.LogWarning("Request timeout occurred. Status code: {StatusCode}", statusCode);
            }
            // If Exception is caught
            // if none of the Exceptions then do the default
            await ModifyHeader(context, title, message, statusCode, errorCode);
        }
    }
    
    private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode, string errorCode = null)
    {
        // display scare-free message to client
        context.Response.ContentType = "application/json";
        
        var problemDetails = new ProblemDetails()
        {
            Detail = message,
            Status = statusCode,
            Title = title
        };
        
        // Add error code if provided
        if (!string.IsNullOrEmpty(errorCode))
        {
            problemDetails.Extensions["errorCode"] = errorCode;
        }
        
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails), CancellationToken.None);
        return;
    }
}
