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
using ResponseCrafter.Helpers;
using ResponseCrafter.HttpExceptions;
using ResponseCrafter.Options;
using static ResponseCrafter.Helpers.ExceptionMessageBuilder;
using IExceptionHandler = Microsoft.AspNetCore.Diagnostics.IExceptionHandler;

namespace ResponseCrafter.ExceptionHandlers.Http;

internal class ApiExceptionHandler : IExceptionHandler
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
         new ConflictException(ExceptionMessages.ConcurrencyMessage.ConvertCase(_convention));
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
         Message = ExceptionMessages.DefaultMessage.ConvertCase(_convention)
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