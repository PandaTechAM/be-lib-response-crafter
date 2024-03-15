using Microsoft.AspNetCore.Mvc;
using PandaTech.IEnumerableFilters.Exceptions;
using ResponseCrafter;
using ResponseCrafter.StandardHttpExceptions;
using ResponseCrafter.Test;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddResponseCrafter();

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

app.MapGet("/error-by-exception-minimal", _ => throw new BadRequestException("some_exception"));
app.MapGet("/error-by-result-minimal", () => Results.BadRequest("some_exception"));


app.MapGet("/token", (HttpContext httpContext) =>
{
    httpContext.SetToken("some_token");
    throw new Exception();
    // return httpContext.GetToken();
});


app.MapGet("/server-error", (Exception) => throw new Exception("some_unhandled_exception"));
app.MapGet("/bad-request", (ApiException) => throw new BadRequestException(errors));
app.MapGet("/unauthorized", (ApiException) => throw new UnauthorizedException());
app.MapGet("/filter", (apiException) => throw new ComparisonNotSupportedException("some_exception")).WithTags("something");

app.MapPost("/load-minimal", async (TestDto dto) =>
{
    throw new MappingException("d");
    await Task.Delay(77);
    return Results.Ok(dto);
});

app.MapControllers();

app.Run();