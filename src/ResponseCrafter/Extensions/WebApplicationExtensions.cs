using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ResponseCrafter.Enums;
using ResponseCrafter.Options;

namespace ResponseCrafter.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder AddResponseCrafter(this WebApplicationBuilder builder,
        NamingConvention namingConvention = NamingConvention.Default)
    {
        builder.Services.AddSingleton(new NamingConventionOptions { NamingConvention = namingConvention });
        builder.Services.AddExceptionHandler<PandaExceptionHandler>();


        return builder;
    }

    public static WebApplication UseResponseCrafter(this WebApplication app)
    {
        app.UseExceptionHandler(_ =>
        {
        }); //the lambda parameter is not needed it is just .net 8 bug which might be fixed in the future

        return app;
    }
}