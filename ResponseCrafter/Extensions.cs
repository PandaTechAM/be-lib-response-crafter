using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ResponseCrafter;

public static class Extensions
{
    public static WebApplicationBuilder AddResponseCrafter(this WebApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<PandaExceptionHandler>();

        return builder;
    }
    public static WebApplication UseResponseCrafter(this WebApplication app)
    {
        app.UseExceptionHandler(_ => { });  //the lambda parameter is not needed it is just .net 8 bug which might be fixed in the future

        return app;
    }
}