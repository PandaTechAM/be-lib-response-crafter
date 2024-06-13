using EFCoreQueryMagic.Exceptions;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using ResponseCrafter.Demo;
using ResponseCrafter.Enums;
using ResponseCrafter.Extensions;
using ResponseCrafter.HttpExceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddResponseCrafter(NamingConvention.ToSnakeCase);

builder.Services.AddControllers();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();


app.UseResponseCrafter();

var errors = new Dictionary<string, string>
{
    { "some_error", "some_error_message" },
    { "some_other_error", "some_other_error_message" }
};

// Test naming convension
app.MapGet("/error-by-exception-to-convert-minimal",
    () => { throw new BadRequestException("Some exception. Try again"); }).ProducesErrorResponse(400, 401, 404);

app.MapGet("/error-by-exception-minimal", _ => throw new BadRequestException("some_exception"));
app.MapGet("/error-by-result-minimal", () => Results.BadRequest("some_exception"));


app.MapGet("/token", (HttpContext httpContext) =>
{
    httpContext.SetToken("some_token");
    throw new Exception();
    // return httpContext.GetToken();
});

app.MapPost("/humanizer", ([FromQuery] string input, [FromQuery] NamingConvention convention) =>
{
    switch (convention)
    {
        case NamingConvention.ToSnakeCase:
            return Results.Ok(input.Underscore());
        case NamingConvention.ToKebabCase:
            return Results.Ok(input.Underscore().Kebaberize());
        case NamingConvention.ToCamelCase:
            return Results.Ok(input.Underscore().Camelize());
        case NamingConvention.ToPascalCase:
            return Results.Ok(input.Underscore().Pascalize());
        case NamingConvention.ToTitleCase:
            return Results.Ok(input.Underscore().Titleize());
        case NamingConvention.ToHumanCase:
            return Results.Ok(input.Underscore().Humanize());
    }

    return Results.Ok(input);
});

app.MapGet("/server-error", (Exception) => throw new Exception("some_unhandled_exception"));
app.MapGet("/bad-request", () => { throw new BadRequestException(errors); });
app.MapGet("/unauthorized", (ApiException) => throw new UnauthorizedException());
app.MapGet("/filter", (apiException) => throw new ComparisonNotSupportedException("some_exception"))
    .WithTags("something");

app.MapPost("/load-minimal", async (TestDto dto) =>
{
    throw new MappingException("d");
    await Task.Delay(77);
    return Results.Ok(dto);
});

app.MapControllers();

app.Run();