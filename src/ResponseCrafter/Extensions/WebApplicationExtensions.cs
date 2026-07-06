using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ResponseCrafter.Enums;
using ResponseCrafter.Options;
using ApiExceptionHandler = ResponseCrafter.ExceptionHandlers.ApiExceptionHandler;

namespace ResponseCrafter.Extensions;

/// <summary>Extension methods for registering and enabling ResponseCrafter on a web application.</summary>
public static class WebApplicationExtensions
{
    /// <summary>Registers ResponseCrafter services and the exception handler with the given naming convention.</summary>
    public static WebApplicationBuilder AddResponseCrafter(this WebApplicationBuilder builder,
        NamingConvention namingConvention = NamingConvention.Default)
    {
        builder.Services.AddSingleton(new NamingConventionOptions
        {
            NamingConvention = namingConvention
        });
        builder.Services.AddExceptionHandler<ApiExceptionHandler>();

        return builder;
    }

    /// <summary>Adds the ResponseCrafter exception-handling middleware to the request pipeline.</summary>
    public static WebApplication UseResponseCrafter(this WebApplication app)
    {
        app.UseExceptionHandler(_ => { });
        return app;
    }
}
