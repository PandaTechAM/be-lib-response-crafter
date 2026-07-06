using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using FluentImporter.Exceptions;
using Gridify;
using GridifyExtensions.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ResponseCrafter.Dtos;
using ResponseCrafter.Enums;
using ResponseCrafter.Extensions;
using ResponseCrafter.Helpers;
using ResponseCrafter.HttpExceptions;
using ResponseCrafter.Options;
using static ResponseCrafter.Helpers.ExceptionMessageBuilder;
using IExceptionHandler = Microsoft.AspNetCore.Diagnostics.IExceptionHandler;

namespace ResponseCrafter.ExceptionHandlers;

internal partial class ApiExceptionHandler : IExceptionHandler
{
    private readonly NamingConvention _convention;
    private readonly ILogger<ApiExceptionHandler> _logger;
    private readonly string _visibility;

    public ApiExceptionHandler(ILogger<ApiExceptionHandler> logger,
        IConfiguration configuration,
        NamingConventionOptions convention)
    {
        _logger = logger;
        _convention = convention.NamingConvention;
        _visibility = configuration.GetResponseCrafterVisibility(_logger);
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case DbUpdateConcurrencyException:
                await HandleDbConcurrencyExceptionAsync(httpContext, cancellationToken);
                break;
            case ImportException targetInvocationException:
                await HandleImportExceptionAsync(httpContext, targetInvocationException, cancellationToken);
                break;
            case GridifyException gridifyException:
                await HandleGridifyExceptionAsync(httpContext, gridifyException, cancellationToken);
                break;
            case GridifyMapperException gridifyMapperException:
                await HandleGridifyExceptionMapperAsync(httpContext, gridifyMapperException, cancellationToken);
                break;
            case BadHttpRequestException badHttpRequestException:
                await HandleBadHttpRequestExceptionAsync(httpContext, badHttpRequestException, cancellationToken);
                break;
            case ApiException apiException:
                await HandleApiExceptionAsync(httpContext, apiException, cancellationToken);
                break;
            default:
                await HandleGeneralExceptionAsync(httpContext, exception, cancellationToken);
                break;
        }

        return true;
    }

    private async Task HandleDbConcurrencyExceptionAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        var exception = new ConflictException(ExceptionMessages.ConcurrencyMessage.ConvertCase(_convention));
        await HandleApiExceptionAsync(httpContext, exception, cancellationToken);
    }

    private async Task HandleImportExceptionAsync(HttpContext httpContext,
        ImportException importException,
        CancellationToken cancellationToken)
    {
        switch (importException)
        {
            case InvalidColumnValueException _:
            case InvalidCellValueException _:
            case InvalidPropertyNameException _:
            case EmptyFileImportException _:
                var mappedException = new BadRequestException(importException.Message.ConvertCase(_convention));
                await HandleApiExceptionAsync(httpContext, mappedException, cancellationToken);
                break;
            default:
                await HandleGeneralExceptionAsync(httpContext, importException, cancellationToken);
                break;
        }
    }

    private async Task HandleBadHttpRequestExceptionAsync(HttpContext httpContext,
        BadHttpRequestException badHttpRequestException,
        CancellationToken ct)
    {
        if (badHttpRequestException.InnerException is JsonException je)
        {
            var errors = new Dictionary<string, string>
            {
                ["path"] = (je.Path ?? "$").ConvertCase(_convention),
                ["detail"] = "json_deserialization_failed".ConvertCase(_convention)
            };

            var posObj = typeof(JsonException).GetProperty(nameof(JsonException.BytePositionInLine))?.GetValue(je);
            if (posObj is long pos and >= 0)
            {
                errors["byte_position"] = pos.ToString(CultureInfo.InvariantCulture);
            }

            var lineObj = typeof(JsonException).GetProperty(nameof(JsonException.LineNumber))?.GetValue(je);
            if (lineObj is long line and >= 0)
            {
                errors["line_number"] = line.ToString(CultureInfo.InvariantCulture);
            }

            var ex = new BadRequestException("invalid_json_payload")
            {
                Errors = errors
            };

            await HandleApiExceptionAsync(httpContext, ex, ct);
            return;
        }

        var generic = new BadRequestException("bad_request_possibly_malformed_json");
        await HandleApiExceptionAsync(httpContext, generic, ct);
    }

    private async Task HandleGridifyExceptionAsync(HttpContext httpContext,
        GridifyException gridifyException,
        CancellationToken cancellationToken)
    {
        var exception = new BadRequestException(gridifyException.Message.ConvertCase(_convention));
        await HandleApiExceptionAsync(httpContext, exception, cancellationToken);
    }

    private async Task HandleGridifyExceptionMapperAsync(HttpContext httpContext,
        GridifyMapperException gridifyMapperException,
        CancellationToken cancellationToken)
    {
        var exception = new BadRequestException(gridifyMapperException.Message.ConvertCase(_convention));
        await HandleApiExceptionAsync(httpContext, exception, cancellationToken);
    }

    private async Task HandleApiExceptionAsync(HttpContext httpContext,
        ApiException exception,
        CancellationToken ct)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
        var instance = CreateRequestPath(httpContext);
        var isServerError = exception.StatusCode >= 500;

        var clientMessage = isServerError && _visibility != "Private"
            ? ExceptionMessages.DefaultMessage.ConvertCase(_convention)
            : exception.Message.ConvertCase(_convention);

        var clientErrors = isServerError && _visibility != "Private"
            ? null
            : exception.Errors.ConvertCase(_convention);

        var response = new ErrorResponse
        {
            RequestId = httpContext.TraceIdentifier,
            TraceId = traceId,
            Instance = instance,
            StatusCode = exception.StatusCode,
            Type = isServerError ? "InternalServerError" : exception.GetType().Name,
            Errors = clientErrors,
            Message = clientMessage
        };

        httpContext.Response.StatusCode = response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(response, ct);

        if (isServerError)
        {
            LogServerError(_logger,
                exception.GetType().Name,
                exception.StatusCode,
                exception.Message,
                instance,
                traceId,
                httpContext.TraceIdentifier,
                exception);
        }
        else
        {
            LogClientError(_logger,
                exception.GetType().Name,
                exception.StatusCode,
                exception.Message,
                instance,
                traceId,
                httpContext.TraceIdentifier,
                exception);
        }
    }

    private async Task HandleGeneralExceptionAsync(HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
        var instance = CreateRequestPath(httpContext);
        var verboseMessage = exception.CreateVerboseExceptionMessage();

        var response = new ErrorResponse
        {
            RequestId = httpContext.TraceIdentifier,
            TraceId = traceId,
            Instance = instance,
            StatusCode = 500,
            Type = "InternalServerError",
            Message = ExceptionMessages.DefaultMessage.ConvertCase(_convention)
        };

        if (_visibility == "Private")
        {
            response.Type = exception.GetType().Name;
            response.Message = verboseMessage.ConvertCase(_convention);
        }

        httpContext.Response.StatusCode = response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        LogUnhandledException(_logger,
            exception.GetType().Name,
            instance,
            traceId,
            httpContext.TraceIdentifier,
            exception);
    }

    // Source-generated logging methods
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message =
            "Server error: {ExceptionType} [{StatusCode}] at {Instance} | TraceId: {TraceId}, RequestId: {RequestId}, Message: {Message}")]
    private static partial void LogServerError(
        ILogger logger,
        string exceptionType,
        int statusCode,
        string message,
        string instance,
        string traceId,
        string requestId,
        Exception exception);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Warning,
        Message =
            "Client error: {ExceptionType} [{StatusCode}] at {Instance} | TraceId: {TraceId}, RequestId: {RequestId}, Message: {Message}")]
    private static partial void LogClientError(
        ILogger logger,
        string exceptionType,
        int statusCode,
        string message,
        string instance,
        string traceId,
        string requestId,
        Exception exception);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Error,
        Message = "Unhandled exception: {ExceptionType} at {Instance} | TraceId: {TraceId}, RequestId: {RequestId}")]
    private static partial void LogUnhandledException(
        ILogger logger,
        string exceptionType,
        string instance,
        string traceId,
        string requestId,
        Exception exception);
}
