using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResponseCrafter.Dtos;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using static ResponseCrafter.ExceptionMessageBuilder;

namespace ResponseCrafter;

public class ResponseCrafterPrivate : IExceptionHandler
{
    private readonly ILogger<ResponseCrafterPrivate> _logger;

    public ResponseCrafterPrivate(ILogger<ResponseCrafterPrivate> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ApiException apiException)
        {
            await HandleApiExceptionAsync(httpContext, apiException, cancellationToken);
        }
        else
        {
            await HandleGeneralExceptionAsync(httpContext, exception, cancellationToken);
        }

        return true;
    }

    private async Task HandleApiExceptionAsync(HttpContext httpContext, ApiException exception,
        CancellationToken cancellationToken)
    {
        
        var response = new ErrorResponse
        {
            StatusCode = exception.StatusCode,
            Type = exception.GetType().Name,
            Message = exception.Message,
            Errors = exception.Errors,
            Instance = CreateRequestPath(httpContext),
            TraceId = httpContext.TraceIdentifier
        };

        httpContext.Response.StatusCode = exception.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken: cancellationToken);

        _logger.LogWarning("API Exception: {Message}", response);
    }

    private async Task HandleGeneralExceptionAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var verboseMessage = CreateVerboseExceptionMessage(exception);

        var response = new ErrorResponse
        {
            StatusCode = (int)HttpStatusCode.InternalServerError,
            Type = exception.GetType().Name,
            Message = verboseMessage,
            Instance = CreateRequestPath(httpContext),
            TraceId = httpContext.TraceIdentifier
        };

        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken: cancellationToken);
        _logger.LogError("API Exception: {Message}", response);
    }
}