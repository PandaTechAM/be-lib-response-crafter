using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResponseCrafter.Dtos;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using static ResponseCrafter.ExceptionMessageBuilder;

namespace ResponseCrafter;

public class ResponseCrafterPublic : IExceptionHandler
{
    private readonly ILogger<ResponseCrafterPublic> _logger;

    public ResponseCrafterPublic(ILogger<ResponseCrafterPublic> logger)
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
            Message = "Something went wrong. Please try again later and/or contact IT support.",
            Instance = CreateRequestPath(httpContext),
            TraceId = httpContext.TraceIdentifier
        };

        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken: cancellationToken);
        _logger.LogError("API Exception: {Message}. Actual hidden message: {Message}", response, verboseMessage);
    }
}