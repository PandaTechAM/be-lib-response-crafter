using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ResponseCrafter;
using ResponseCrafter.Demo.Hubs;
using ResponseCrafter.Enums;
using ResponseCrafter.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddResponseCrafter(NamingConvention.ToSnakeCase);

builder.Services.AddSignalR(o => o.AddFilter<SignalRExceptionFilter>());

builder.Services.AddControllers();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapHub<ChatHub>("/chat-hub");


app.UseResponseCrafter();



app.MapPost("/humanizer",
   ([FromQuery] string input, [FromQuery] NamingConvention convention) =>
   {
      switch (convention)
      {
         case NamingConvention.ToSnakeCase:
            return Results.Ok(input.Underscore());
         case NamingConvention.ToKebabCase:
            return Results.Ok(input.Underscore()
                                   .Kebaberize());
         case NamingConvention.ToCamelCase:
            return Results.Ok(input.Underscore()
                                   .Camelize());
         case NamingConvention.ToPascalCase:
            return Results.Ok(input.Underscore()
                                   .Pascalize());
         case NamingConvention.ToTitleCase:
            return Results.Ok(input.Underscore()
                                   .Titleize());
         case NamingConvention.ToHumanCase:
            return Results.Ok(input.Underscore()
                                   .Humanize());
      }

      return Results.Ok(input);
   });




app.MapControllers();

app.Run();