using System.Diagnostics;
using FluentImporter.Exceptions;
using GridifyExtensions.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ResponseCrafter.Dtos;
using ResponseCrafter.Enums;
using ResponseCrafter.Extensions;
using ResponseCrafter.HttpExceptions;
using ResponseCrafter.Options;
using ServiceResponseCrafter.Dtos;
using ServiceResponseCrafter.ExceptionHandler;
using static ResponseCrafter.Helpers.ExceptionMessageBuilder;
using IExceptionHandler = Microsoft.AspNetCore.Diagnostics.IExceptionHandler;

namespace ResponseCrafter;

public class ApiExceptionHandler : IExceptionHandler
{
   private const string DefaultMessage = "something_went_wrong_please_try_again_later_and_or_contact_it_support";

   private const string ConcurrencyMessage =
      "a_concurrency_conflict_occurred._please_reload_the_resource_and_try_you_update_again";

   private readonly NamingConvention _convention;
   private readonly ILogger<ApiExceptionHandler> _logger;
   private readonly string _visibility;

   public ApiExceptionHandler(ILogger<ApiExceptionHandler> logger,
      IConfiguration configuration,
      NamingConventionOptions convention)
   {
      _logger = logger;
      _convention = convention.NamingConvention;
      _visibility = configuration["ResponseCrafterVisibility"]!;


      if (string.IsNullOrWhiteSpace(_visibility) || (_visibility != "Private" && _visibility != "Public"))
      {
         _visibility = "Public";
         _logger.LogWarning("Visibility configuration was not available. Defaulted to 'Public'.");
      }
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
         case ServiceException serviceException:
            await HandleServiceExceptionAsync(httpContext, serviceException, cancellationToken);
            break;
         case GridifyException gridifyException:
            await HandleGridifyExceptionAsync(httpContext, gridifyException, cancellationToken);
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
      var exception =
         new ConflictException(ConcurrencyMessage.ConvertCase(_convention));
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

   private async Task HandleServiceExceptionAsync(HttpContext httpContext,
      ServiceException serviceException,
      CancellationToken cancellationToken)
   {
      var originalMessage = serviceException.Message.ConvertCase(_convention);

      var response = new ServiceResponse
      {
         Message = originalMessage,
         ResponseStatus = serviceException.ResponseStatus,
         Success = false
      };

      httpContext.Response.StatusCode = (int)serviceException.ResponseStatus;

      if (_visibility == "Public")
      {
         if (httpContext.Response.StatusCode >= 500)
         {
            response.Message = DefaultMessage.ConvertCase(_convention);
         }
      }


      if (httpContext.Response.StatusCode >= 500)
      {
         _logger.LogError("ServiceException encountered: {ExceptionMessage}", originalMessage);
      }
      else
      {
         _logger.LogWarning("ServiceException encountered: {ExceptionMessage}", originalMessage);
      }

      await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
   }


   private async Task HandleGridifyExceptionAsync(HttpContext httpContext,
      GridifyException gridifyException,
      CancellationToken cancellationToken)
   {
      var exception = new BadRequestException(gridifyException.Message.ConvertCase(_convention));
      await HandleApiExceptionAsync(httpContext, exception, cancellationToken);
   }

   private async Task HandleApiExceptionAsync(HttpContext httpContext,
      ApiException exception,
      CancellationToken cancellationToken)
   {
      var response = new ErrorResponse
      {
         RequestId = httpContext.TraceIdentifier,
         TraceId = Activity.Current?.RootId ?? "",
         Instance = CreateRequestPath(httpContext),
         StatusCode = exception.StatusCode,
         Type = exception.GetType()
                         .Name,
         Errors = exception.Errors,
         Message = exception.Message.ConvertCase(_convention)
      };

      httpContext.Response.StatusCode = exception.StatusCode;
      await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

      if (response.Errors is null || response.Errors.Count == 0)
      {
         _logger.LogWarning("ApiException encountered: {Message}", response.Message);
      }
      else
      {
         _logger.LogWarning("ApiException encountered: {Message} with errors: {@Errors}",
            response.Message,
            response.Errors);
      }
   }

   private async Task HandleGeneralExceptionAsync(HttpContext httpContext,
      Exception exception,
      CancellationToken cancellationToken)
   {
      var verboseMessage = exception.CreateVerboseExceptionMessage();

      var response = new ErrorResponse
      {
         RequestId = httpContext.TraceIdentifier,
         TraceId = Activity.Current?.RootId ?? "",
         Instance = CreateRequestPath(httpContext),
         StatusCode = 500,
         Type = "InternalServerError",
         Message = DefaultMessage.ConvertCase(_convention)
      };

      if (_visibility == "Private")
      {
         response.Type = exception.GetType()
                                  .Name;
         response.Message = verboseMessage.ConvertCase(_convention);
      }

      httpContext.Response.StatusCode = response.StatusCode;
      await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);


      _logger.LogError("Unhandled exception encountered: {Message}", verboseMessage);
   }
}