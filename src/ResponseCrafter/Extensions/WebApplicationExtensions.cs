using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ResponseCrafter.Enums;

namespace ResponseCrafter.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder AddResponseCrafter(this WebApplicationBuilder builder, NamingConvention? namingConvention = null)
    {
        builder.Services.AddExceptionHandler<PandaExceptionHandler>();
        
        switch (namingConvention)
        {
            case null:
            case NamingConvention.Default:
                builder.Services.AddSingleton<Func<string, string>>(NamingConventionExtensions.Default);
                break;
            case NamingConvention.SnakeCaseLower:
                builder.Services.AddSingleton<Func<string, string>>(NamingConventionExtensions.ToSnakeCaseLowerNamingConvention);
                break;
            case NamingConvention.SnakeCaseUpper:
                builder.Services.AddSingleton<Func<string, string>>(NamingConventionExtensions.ToSnakeCaseUpperNamingConvention);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(namingConvention), namingConvention, null);
        }

        return builder;
    }
    public static WebApplication UseResponseCrafter(this WebApplication app)
    {
        app.UseExceptionHandler(_ => { });  //the lambda parameter is not needed it is just .net 8 bug which might be fixed in the future

        return app;
    }
}