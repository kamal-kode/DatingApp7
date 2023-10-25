using System.Net;
using System.Text.Json;
using API.Errors;
using SQLitePCL;

namespace API.Middleware;

/// <summary>
/// This middleware will handle all the exception from the controller/action
/// </summary>
public class ExceptionMiddleware
{
    private const string  _internalServerError= "Internal server Error";
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _hostEnvironment;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment hostEnvironment)
    {
        _next = next;
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            //Return to client.
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var response= _hostEnvironment.IsDevelopment() ? new ApiException(context.Response.StatusCode,ex.Message, ex.StackTrace?.ToString())
            : new ApiException(context.Response.StatusCode,ex.Message, _internalServerError);
            
            var options = new JsonSerializerOptions{
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var json= JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}
