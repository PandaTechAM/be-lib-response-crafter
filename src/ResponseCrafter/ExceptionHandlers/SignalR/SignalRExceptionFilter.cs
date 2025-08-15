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

public class SignalRExceptionFilter : IHubFilter
{
   private readonly NamingConvention _convention;
   private readonly ILogger<SignalRExceptionFilter> _logger;
   private readonly string _visibility;

   public SignalRExceptionFilter(ILogger<SignalRExceptionFilter> logger,
      IConfiguration configuration,
      NamingConventionOptions convention)
   {
      _logger = logger;
      _convention = convention.NamingConvention;
      _visibility = configuration.GetResponseCrafterVisibility(_logger);
   }


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
            return ha.InvocationId;
      }

      // 3) Optional non-breaking fallback: header/query (safe to keep off if you don’t want it)
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

      using (_logger.BeginScope(new Dictionary<string, object>
             {
                ["trace_id"] = traceId,
                ["hub"] = ctx.Hub.GetType()
                             .Name,
                ["method"] = ctx.HubMethodName,
                ["connection_id"] = ctx.Context.ConnectionId,
                ["user_id"] = ctx.Context.UserIdentifier ?? "",
                ["invocation_id"] = invocationId,
                ["status_code"] = ex.StatusCode
             }))
      {
         var response = new HubErrorResponse
         {
            TraceId = traceId,
            InvocationId = invocationId,
            Instance = ctx.HubMethodName,
            StatusCode = ex.StatusCode,
            Message = ex.Message.ConvertCase(_convention),
            Errors = ex.Errors.ConvertCase(_convention)
         };

         if (response.Errors is null || response.Errors.Count == 0)
         {
            _logger.LogWarning("SignalR exception: {Message}", response.Message);
         }
         else
         {
            _logger.LogWarning("SignalR exception: {Message} with errors: {@Errors}",
               response.Message,
               response.Errors);
         }

         await ctx.Hub.Clients.Caller.SendAsync("ReceiveError", response, ctx.Context.ConnectionAborted);
      }
   }

   private async Task HandleGeneralExceptionAsync(HubInvocationContext ctx,
      Exception ex,
      string invocationId)
   {
      var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
      var verbose = ex.CreateVerboseExceptionMessage();

      using (_logger.BeginScope(new Dictionary<string, object>
             {
                ["trace_id"] = traceId,
                ["hub"] = ctx.Hub.GetType()
                             .Name,
                ["method"] = ctx.HubMethodName,
                ["connection_id"] = ctx.Context.ConnectionId,
                ["user_id"] = ctx.Context.UserIdentifier ?? "",
                ["invocation_id"] = invocationId,
                ["status_code"] = 500
             }))
      {
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

         _logger.LogError("Unhandled SignalR exception: {Message}", verbose);

         await ctx.Hub.Clients.Caller.SendAsync("ReceiveError", response, ctx.Context.ConnectionAborted);
      }
   }

   private static object? NullResultFor(HubInvocationContext _)
   {
      // For Task/void: no payload is emitted.
      // For Task<T>/T: caller receives null once (plus ReceiveError event).
      return null;
   }
}