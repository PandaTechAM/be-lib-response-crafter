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

namespace ResponseCrafter;

public class SignalRExceptionFilter : IHubFilter
{
   private readonly ILogger<SignalRExceptionFilter> _logger;
   private readonly NamingConvention _convention;
   private readonly string _visibility;
   private const string DefaultMessage = "something_went_wrong_please_try_again_later_and_or_contact_it_support";

   private const string ConcurrencyMessage =
      "a_concurrency_conflict_occurred._please_reload_the_resource_and_try_you_update_again";
   
   public SignalRExceptionFilter(ILogger<SignalRExceptionFilter> logger, IConfiguration configuration,
      NamingConventionOptions convention)
   {
      _logger = logger;
      _convention = convention.NamingConvention;
      _visibility = configuration["ResponseCrafterVisibility"]!;


      if (string.IsNullOrWhiteSpace(_visibility) || _visibility != "Private" && _visibility != "Public")
      {
         _visibility = "Public";
         _logger.LogWarning("Visibility configuration was not available. Defaulted to 'Public'.");
      }
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
         var exception = new ConflictException(ConcurrencyMessage.ConvertCase(_convention));
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

   private static string TryGetInvocationId(HubInvocationContext hubInvocationContext)
   {
      if (hubInvocationContext.HubMethodArguments.Count != 1)
      {
         throw new BadRequestException("Invalid hub method arguments. Expected a single HubArgument<T> parameter.");
      }

      var hubArgument = hubInvocationContext.HubMethodArguments[0];
      var invocationIdProperty = hubArgument?.GetType().GetProperty("InvocationId");

      if (invocationIdProperty == null || invocationIdProperty.PropertyType != typeof(string))
      {
         throw new BadRequestException("Invalid hub method argument. Missing 'InvocationId' property.");
      }

      var invocationId = invocationIdProperty.GetValue(hubArgument) as string;
      if (string.IsNullOrWhiteSpace(invocationId))
      {
         throw new BadRequestException("Invocation ID cannot be null, empty, or whitespace.");
      }

      return invocationId;
   }

   private async Task<HubErrorResponse> HandleApiExceptionAsync(HubInvocationContext invocationContext, ApiException exception, string invocationId)
   {
      var response = new HubErrorResponse
      {
         TraceId = Activity.Current?.RootId ?? "",
         InvocationId = invocationId,
         Instance = invocationContext.HubMethodName,
         StatusCode = exception.StatusCode,
         Message = exception.Message.ConvertCase(_convention),
         Errors = exception.Errors,
      };
   
      if (response.Errors is null || response.Errors.Count == 0)
      {
         _logger.LogWarning("SignalR Exception Encountered: {Message}", response.Message);
      }
      else
      {
         _logger.LogWarning("SignalR Exception Encountered: {Message} with errors: {@Errors}", response.Message,
            response.Errors);
      }
      
      await invocationContext.Hub.Clients.Caller.SendAsync("ReceiveError", response);

      return response;
   }

   private async Task<HubErrorResponse> HandleGeneralExceptionAsync(HubInvocationContext invocationContext, Exception exception, string invocationId)
   {
      var verboseMessage = exception.CreateVerboseExceptionMessage();
      
      var response = new HubErrorResponse
      {
         TraceId = Activity.Current?.RootId ?? "",
         InvocationId = invocationId,
         Instance = invocationContext.HubMethodName,
         StatusCode = 500,
         Message = DefaultMessage.ConvertCase(_convention)
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
