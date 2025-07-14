using LeaveManagement.Application.Wrappers;
using System.Net;
using System.Text.Json;

namespace LeaveManagement.API.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            var responseModel = new Response<string>() { Succeeded = false, Message = error?.Message };

            switch (error)
            {
                case LeaveManagement.Application.Exceptions.ApiException e:
                    // custom application error
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    _logger.LogError($"Stack Trace: {e.StackTrace} Inner Exception {e.InnerException} Message:{e.Message}");
                    break;
                case LeaveManagement.Application.Exceptions.ValidationException e:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel.Errors = e.Errors
                        .SelectMany(kvp => kvp.Value.Select(errorMsg =>
                            $"{kvp.Key}: {errorMsg}"))
                        .ToList();
                    responseModel.Message = string.Join(", ", responseModel.Errors);
                    _logger.LogError($"Validation Errors: {string.Join(", ", responseModel.Errors)}");
                    break;
                case KeyNotFoundException e:
                    // not found error
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    _logger.LogError($"Stack Trace: {e.StackTrace} Inner Exception {e.InnerException} Message:{e.Message}");
                    break;
                default:
                    // unhandled error
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    //_logger.LogError($"Stack Trace: {e.StackTrace} Inner Exception {e.InnerException} Message:{e.Message}");
                    break;
            }

            var result = JsonSerializer.Serialize(responseModel);

            await response.WriteAsync(result);
        }
    }
}