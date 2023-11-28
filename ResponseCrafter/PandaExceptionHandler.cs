using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResponseCrafter.Dtos;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using static ResponseCrafter.ExceptionMessageBuilder;

namespace ResponseCrafter;

public class PandaExceptionHandler : IExceptionHandler
{
    private readonly ILogger<PandaExceptionHandler> _logger;
    private readonly string _visibility;

    public PandaExceptionHandler(ILogger<PandaExceptionHandler> logger, IConfiguration configuration)
    {
        _logger = logger;
        _visibility = configuration["ResponseCrafterVisibility"]!;

        if (string.IsNullOrEmpty(_visibility) || _visibility != "Private" && _visibility != "Public")
        {
            _visibility = "Public";
            _logger.LogWarning("Visibility configuration was not available. Defaulting to `Public`.");
        }
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
            TraceId = httpContext.TraceIdentifier,
            Instance = CreateRequestPath(httpContext),
            StatusCode = exception.StatusCode,
            Type = exception.GetType().Name,
            Errors = exception.Errors,
            Message = exception.Message
        };

        httpContext.Response.StatusCode = exception.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken: cancellationToken);

        _logger.LogWarning("API Exception: {Response}", response);
    }

    private async Task HandleGeneralExceptionAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var verboseMessage = CreateVerboseExceptionMessage(exception);

        var response = new ErrorResponse
        {
            TraceId = httpContext.TraceIdentifier,
            Instance = CreateRequestPath(httpContext),
            StatusCode = 500,
            Type = "InternalServerError",
            Message = "something_went_wrong_please_try_again_later_and_or_contact_it_support"
        };

        if (_visibility == "Private")
        {
            response.Type = exception.GetType().Name;
            response.Message = verboseMessage;
        }

        httpContext.Response.StatusCode = response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken: cancellationToken);

        if (_visibility == "Public")
            _logger.LogError("API Exception: {Response}. Actual hidden message: {Message}", response, verboseMessage);
        else
            _logger.LogError("API Exception: {Response}", response);
    }
}