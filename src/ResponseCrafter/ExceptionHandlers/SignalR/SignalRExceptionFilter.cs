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
         invocationId = TryGetInvocationId<IHubArgument>(invocationContext);
         return await next(invocationContext);
      }
      catch (DbUpdateConcurrencyException)
      {
         var exception = new ConflictException(ExceptionMessages.ConcurrencyMessage.ConvertCase(_convention));
         return await HandleApiExceptionAsync(invocationContext, exception, invocationId);
      }
      catch (GridifyException ex)
      {
         var exception = new BadRequestException(ex.Message.ConvertCase(_convention));
         return await HandleApiExceptionAsync(invocationContext, exception, invocationId);
      }
      catch (ApiException ex)
      {
         return await HandleApiExceptionAsync(invocationContext, ex, invocationId);
      }
      catch (Exception ex)
      {
         return await HandleGeneralExceptionAsync(invocationContext, ex, invocationId);
      }
   }

   private static string TryGetInvocationId<T>(HubInvocationContext hubInvocationContext) where T : IHubArgument
   {
      if (hubInvocationContext.HubMethodArguments is not [T hubArgument])
      {
         throw new BadRequestException("Invalid hub method arguments. Request model does not implement IHubArgument interface.");
      }

      var invocationId = hubArgument.InvocationId;
      if (string.IsNullOrWhiteSpace(invocationId))
      {
         throw new BadRequestException("Invocation ID cannot be null, empty, or whitespace.");
      }

      return invocationId;
   }

   private async Task<HubErrorResponse> HandleApiExceptionAsync(HubInvocationContext invocationContext,
      ApiException exception,
      string invocationId)
   {
      var response = new HubErrorResponse
      {
         TraceId = Activity.Current?.RootId ?? "",
         InvocationId = invocationId,
         Instance = invocationContext.HubMethodName,
         StatusCode = exception.StatusCode,
         Message = exception.Message.ConvertCase(_convention),
         Errors = exception.Errors
      };

      if (response.Errors is null || response.Errors.Count == 0)
      {
         _logger.LogWarning("SignalR Exception Encountered: {Message}", response.Message);
      }
      else
      {
         _logger.LogWarning("SignalR Exception Encountered: {Message} with errors: {@Errors}",
            response.Message,
            response.Errors);
      }

      await invocationContext.Hub.Clients.Caller.SendAsync("ReceiveError", response);

      return response;
   }

   private async Task<HubErrorResponse> HandleGeneralExceptionAsync(HubInvocationContext invocationContext,
      Exception exception,
      string invocationId)
   {
      var verboseMessage = exception.CreateVerboseExceptionMessage();

      var response = new HubErrorResponse
      {
         TraceId = Activity.Current?.RootId ?? "",
         InvocationId = invocationId,
         Instance = invocationContext.HubMethodName,
         StatusCode = 500,
         Message = ExceptionMessages.DefaultMessage.ConvertCase(_convention)
      };

      if (_visibility == "Private")
      {
         response.Message = verboseMessage.ConvertCase(_convention);
      }

      _logger.LogError("Unhandled exception encountered: {Message}", verboseMessage);

      await invocationContext.Hub.Clients.Caller.SendAsync("ReceiveError", response);

      return response;
   }
}