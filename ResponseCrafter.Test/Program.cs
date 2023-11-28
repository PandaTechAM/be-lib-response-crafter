using ResponseCrafter;
using ResponseCrafter.Dtos;
using ResponseCrafter.StandardHttpExceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<PandaExceptionHandler>();
var app = builder.Build();
app.UseSwagger();

app.UseHttpsRedirection();

app.UseExceptionHandler(_ => { });

var errors = new Dictionary<string, string>
{
    {"some_error", "some_error_message"},
    {"some_other_error", "some_other_error_message"}
};

app.MapGet("/server-error", (Exception) => throw new Exception("some_unhandled_exception"));
app.MapGet("/bad-request", (ApiException) => throw new BadRequestException(errors));
app.MapGet("/unauthorized", (ApiException) => throw new UnauthorizedException());
app.MapGet("/conflict", (ApiException) => throw new ConflictException("some_exception"));

app.Run();

