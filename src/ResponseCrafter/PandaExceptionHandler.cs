using EFCoreQueryMagic.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResponseCrafter.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PandaTech.ServiceResponse;
using ResponseCrafter.StandardHttpExceptions;
using static ResponseCrafter.ExceptionMessageBuilder;
using IExceptionHandler = Microsoft.AspNetCore.Diagnostics.IExceptionHandler;

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
            _logger.LogWarning("Visibility configuration was not available. Defaulted to 'Public'.");
        }
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case ApiException apiException:
                await HandleApiExceptionAsync(httpContext, apiException, cancellationToken);
                break;
            case DbUpdateConcurrencyException:
                await HandleDbConcurrencyExceptionAsync(httpContext, cancellationToken);
                break;
            case FilterException filterException:
                await HandleFilterExceptionAsync(httpContext, filterException, cancellationToken);
                break;
            case ServiceException serviceException:
                await HandleServiceExceptionAsync(httpContext, serviceException, cancellationToken);
                break;
            default:
                await HandleGeneralExceptionAsync(httpContext, exception, cancellationToken);
                break;
        }

        return true;
    }

    private async Task HandleServiceExceptionAsync(HttpContext httpContext, ServiceException serviceException,
        CancellationToken cancellationToken)
    {
        var response = new ServiceResponse
        {
            Message = "a_concurrency_conflict_occurred._please_reload_the_resource_and_try_you_update_again",
            ResponseStatus = serviceException.ResponseStatus,
            Success = false
        };

        if (_visibility == "Private")
        {
            response.Message = serviceException.Message;
        }

        httpContext.Response.StatusCode = (int)serviceException.ResponseStatus;

        if (httpContext.Response.StatusCode >= 500)
        {
            _logger.LogError("ServiceException encountered: {ExceptionMessage}", serviceException.Message);
        }
        else
        {
            _logger.LogWarning("ServiceException encountered: {ExceptionMessage}", serviceException.Message);
        }

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
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

        if (response.Errors is null || response.Errors.Count == 0)
        {
            _logger.LogWarning("ApiException encountered: {Message}", response.Message);
        }
        else
        {
            _logger.LogWarning("ApiException encountered: {Message} with errors: {@Errors}", response.Message,
                response.Errors);
        }
    }

    private async Task HandleDbConcurrencyExceptionAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        var exception =
            new ConflictException(
                "a_concurrency_conflict_occurred._please_reload_the_resource_and_try_you_update_again");
        await HandleApiExceptionAsync(httpContext, exception, cancellationToken);
    }

    private async Task HandleFilterExceptionAsync(HttpContext httpContext, FilterException filterException,
        CancellationToken cancellationToken)
    {
        switch (filterException)
        {
            case ComparisonNotSupportedException _:
            case MappingException _:
            case NoOrderingFoundException _:
            case PropertyNotFoundException _:
            case UnsupportedFilterException _:
                var exceptionName = filterException.GetType().Name;
                var formattedMessage = $"{exceptionName} in Filters: {filterException.Message}";
                var mappedException = new BadRequestException(formattedMessage);
                await HandleApiExceptionAsync(httpContext, mappedException, cancellationToken);
                break;
            default:
                await HandleGeneralExceptionAsync(httpContext, filterException, cancellationToken);
                break;
        }
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


        _logger.LogError("Unhandled exception encountered: {Message}", verboseMessage);
    }
}