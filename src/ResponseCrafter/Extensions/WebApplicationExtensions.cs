using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ResponseCrafter.Enums;
using ResponseCrafter.ExceptionHandlers.Http;
using ResponseCrafter.Options;

namespace ResponseCrafter.Extensions;

public static class WebApplicationExtensions
{
   public static WebApplicationBuilder AddResponseCrafter(this WebApplicationBuilder builder,
      NamingConvention namingConvention = NamingConvention.Default,
      bool suppressExceptionHandlerMiddlewareLog  = true)
   {
      builder.Services.AddSingleton(new NamingConventionOptions
      {
         NamingConvention = namingConvention
      });
      builder.Services.AddExceptionHandler<ApiExceptionHandler>();

      if (suppressExceptionHandlerMiddlewareLog )
      {
         builder.Logging.AddFilter(
            "Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware",
            LogLevel.None);
      }


      return builder;
   }

   public static WebApplication UseResponseCrafter(this WebApplication app)
   {
      app.UseExceptionHandler(_ =>
      {
      });
      return app;
   }
}