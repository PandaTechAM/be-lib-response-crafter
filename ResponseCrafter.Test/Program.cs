using ResponseCrafter;
using ResponseCrafter.Dtos;
using ResponseCrafter.StandardHttpExceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<ResponseCrafterPublic>();
var app = builder.Build();
app.UseSwagger();

app.UseHttpsRedirection();

app.UseExceptionHandler(_ => { });

var errors = new List<ErrorDetail>
{
    new("Error1", "Error2"),
    new("Error3", "Error4")
};

app.MapGet("/server-error", (Exception) => throw new Exception("some_unhandled_exception"));
app.MapGet("/bad-request", (ApiException) => throw new BadRequestException(errors));
app.MapGet("/unauthorized", (ApiException) => throw new UnauthorizedException());
app.MapGet("/conflict", (ApiException) => throw new ConflictException("some_exception"));

app.Run();

