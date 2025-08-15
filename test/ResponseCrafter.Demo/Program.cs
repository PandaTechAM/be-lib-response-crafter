using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ResponseCrafter.Demo.Hubs;
using ResponseCrafter.Enums;
using ResponseCrafter.ExceptionHandlers.SignalR;
using ResponseCrafter.Extensions;
using ResponseCrafter.HttpExceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddResponseCrafter(NamingConvention.ToSnakeCase);


builder.Services.AddSignalR(o => o.AddFilter<SignalRExceptionFilter>());

// HTTP client for demo failures
builder.Services.AddHttpClient("demo-http",
   c =>
   {
      c.BaseAddress = new Uri("http://localhost:65535"); // guaranteed to refuse
      c.Timeout = TimeSpan.FromSeconds(2);
   });


builder.Services.AddControllers();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapHub<ChatHub>("/chat-hub");


app.UseResponseCrafter();


app.MapPost("/errors/bad-request",
   () =>
   {
      throw new BadRequestException("invalid_payload",
         new Dictionary<string, string>
         {
            ["email"] = "email_address_is_not_in_a_valid_format",
            ["password"] = "password_must_be_at_least_8_characters_long"
         });
   });

app.MapPost("/errors/dotnet",
   () =>
   {
      throw new InvalidOperationException("simulated_invalid_operation");
   });

app.MapPost("/errors/500",
   () =>
   {
      throw new Exception("simulated_unhandled_exception");
   });

app.MapGet("/errors/httpclient",
   async ([FromServices] IHttpClientFactory factory) =>
   {
      throw new BadRequestException("something wrong");
      var http = factory.CreateClient("demo-http");
      // Will throw HttpRequestException (connection refused) or TaskCanceledException on timeout
      _ = await http.GetStringAsync("/");
      return Results.Ok("should_not_reach_here");
   });


app.MapControllers();

app.Run();