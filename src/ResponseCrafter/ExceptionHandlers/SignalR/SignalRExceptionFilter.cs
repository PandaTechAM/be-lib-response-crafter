using System.Diagnostics;
using GridifyExtensions.Exceptions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ResponseCrafter.Dtos;
using ResponseCrafter.Enums;
using ResponseCrafter.Extensions;
using ResponseCrafter.Helpers;
using ResponseCrafter.HttpExceptions;
using ResponseCrafter.Options;

namespace ResponseCrafter.ExceptionHandlers.SignalR;

/// <summary>
///     Hub filter for handling exceptions in SignalR hubs and broadcasting error responses to clients.
/// </summary>
public partial class SignalRExceptionFilter : IHubFilter
{
    private readonly NamingConvention _convention;
    private readonly ILogger<SignalRExceptionFilter> _logger;
    private readonly string _visibility;

    /// <summary>Initializes a new <see cref="SignalRExceptionFilter" />.</summary>
    public SignalRExceptionFilter(ILogger<SignalRExceptionFilter> logger,
        IConfiguration configuration,
        NamingConventionOptions convention)
    {
        _logger = logger;
        _convention = convention.NamingConvention;
        _visibility = configuration.GetResponseCrafterVisibility(_logger);
    }

    /// <summary>Invokes the hub method and converts any thrown exception into an error response sent to the caller.</summary>
    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var invocationId = "";

        try
        {
            invocationId = TryGetInvocationId(invocationContext);
            return await next(invocationContext);
        }
        catch (DbUpdateConcurrencyException)
        {
            var exception = new ConflictException(ExceptionMessages.ConcurrencyMessage.ConvertCase(_convention));
            await HandleApiExceptionAsync(invocationContext, exception, invocationId);
            return NullResultFor(invocationContext);
        }
        catch (GridifyException ex)
        {
            var exception = new BadRequestException(ex.Message.ConvertCase(_convention));
            await HandleApiExceptionAsync(invocationContext, exception, invocationId);
            return NullResultFor(invocationContext);
        }
        catch (ApiException ex)
        {
            await HandleApiExceptionAsync(invocationContext, ex, invocationId);
            return NullResultFor(invocationContext);
        }
        catch (Exception ex)
        {
            await HandleGeneralExceptionAsync(invocationContext, ex, invocationId);
            return NullResultFor(invocationContext);
        }
    }

    private static string TryGetInvocationId(HubInvocationContext ctx)
    {
        // 1) Legacy behavior: exactly one arg that implements IHubArgument
        if (ctx.HubMethodArguments is [IHubArgument single] &&
            !string.IsNullOrWhiteSpace(single.InvocationId))
        {
            return single.InvocationId;
        }

        // 2) New tolerant path: scan all args for first valid IHubArgument
        foreach (var arg in ctx.HubMethodArguments)
        {
            if (arg is IHubArgument ha && !string.IsNullOrWhiteSpace(ha.InvocationId))
            {
                return ha.InvocationId;
            }
        }

        // 3) Optional non-breaking fallback: header/query (safe to keep off if you don't want it)
        var http = ctx.Context.GetHttpContext();
        var id = http?.Request
                     .Headers["x-invocation-id"]
                     .FirstOrDefault()
                 ?? http?.Request
                     .Query["invocation_id"]
                     .FirstOrDefault();

        return !string.IsNullOrWhiteSpace(id)
            ? id
            : throw new BadRequestException("Invocation ID cannot be null, empty, or whitespace.");
    }

    private async Task HandleApiExceptionAsync(HubInvocationContext ctx,
        ApiException ex,
        string invocationId)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
        var isServerError = ex.StatusCode >= 500;

        var clientMessage = isServerError && _visibility != "Private"
            ? ExceptionMessages.DefaultMessage.ConvertCase(_convention)
            : ex.Message.ConvertCase(_convention);

        var clientErrors = isServerError && _visibility != "Private"
            ? null
            : ex.Errors.ConvertCase(_convention);

        var response = new HubErrorResponse
        {
            TraceId = traceId,
            InvocationId = invocationId,
            Instance = ctx.HubMethodName,
            StatusCode = ex.StatusCode,
            Message = clientMessage,
            Errors = clientErrors
        };

        if (isServerError)
        {
            LogServerError(_logger,
                ex.GetType()
                    .Name,
                ex.StatusCode,
                ctx.HubMethodName,
                ctx.Hub.GetType()
                    .Name,
                invocationId,
                traceId,
                ex);
        }
        else
        {
            LogClientError(_logger,
                ex.GetType()
                    .Name,
                ex.StatusCode,
                ctx.HubMethodName,
                ctx.Hub.GetType()
                    .Name,
                invocationId,
                traceId,
                ex);
        }

        await ctx.Hub.Clients.Caller.SendAsync("ReceiveError", response, ctx.Context.ConnectionAborted);
    }

    private async Task HandleGeneralExceptionAsync(HubInvocationContext ctx,
        Exception ex,
        string invocationId)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
        var verbose = ex.CreateVerboseExceptionMessage();

        var response = new HubErrorResponse
        {
            TraceId = traceId,
            InvocationId = invocationId,
            Instance = ctx.HubMethodName,
            StatusCode = 500,
            Message = _visibility == "Private"
                ? verbose.ConvertCase(_convention)
                : ExceptionMessages.DefaultMessage.ConvertCase(_convention)
        };

        LogUnhandledException(_logger,
            ex.GetType()
                .Name,
            ctx.HubMethodName,
            ctx.Hub.GetType()
                .Name,
            invocationId,
            traceId,
            ex);

        await ctx.Hub.Clients.Caller.SendAsync("ReceiveError", response, ctx.Context.ConnectionAborted);
    }

    private static object? NullResultFor(HubInvocationContext _)
    {
        // For Task/void: no payload is emitted.
        // For Task<T>/T: caller receives null once (plus ReceiveError event).
        return null;
    }

    // Source-generated logging methods
    [LoggerMessage(
        EventId = 10,
        Level = LogLevel.Error,
        Message =
            "SignalR server error: {ExceptionType} [{StatusCode}] in {HubName}.{MethodName} | InvocationId: {InvocationId}, TraceId: {TraceId}")]
    private static partial void LogServerError(ILogger logger,
        string exceptionType,
        int statusCode,
        string methodName,
        string hubName,
        string invocationId,
        string traceId,
        Exception exception);

    [LoggerMessage(
        EventId = 11,
        Level = LogLevel.Warning,
        Message =
            "SignalR client error: {ExceptionType} [{StatusCode}] in {HubName}.{MethodName} | InvocationId: {InvocationId}, TraceId: {TraceId}")]
    private static partial void LogClientError(ILogger logger,
        string exceptionType,
        int statusCode,
        string methodName,
        string hubName,
        string invocationId,
        string traceId,
        Exception exception);

    [LoggerMessage(
        EventId = 12,
        Level = LogLevel.Error,
        Message =
            "SignalR unhandled exception: {ExceptionType} in {HubName}.{MethodName} | InvocationId: {InvocationId}, TraceId: {TraceId}")]
    private static partial void LogUnhandledException(ILogger logger,
        string exceptionType,
        string methodName,
        string hubName,
        string invocationId,
        string traceId,
        Exception exception);
}
