using Microsoft.AspNetCore.SignalR;
using ResponseCrafter.ExceptionHandlers.SignalR;
using ResponseCrafter.HttpExceptions;

namespace ResponseCrafter.Demo.Hubs;

public class ChatHub : Hub<IChatClient>
{
   public override async Task OnConnectedAsync()
   {
      await Clients.All.ReceiveMessage($"{Context.ConnectionId} joined the chat");
      await base.OnConnectedAsync();
   }

   public Task SendMessage(Message hubArgument)
   {
      throw new BadRequestException("invalid_message_format",
         new Dictionary<string, string>
         {
            ["content"] = "message_content_is_invalid"
         });
   }

   // 4b) Plain .NET exception -> 500 path in SignalR filter
   public Task Boom(Message hubArgument)
   {
      throw new InvalidOperationException("signalr_method_failed");
   }
}

public class Message : IHubArgument
{
   public required string User { get; set; }
   public required string Content { get; set; }
   public required string InvocationId { get; set; }
}