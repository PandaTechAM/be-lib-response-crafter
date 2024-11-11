using Microsoft.AspNetCore.SignalR;
using ResponseCrafter.Helpers;
using ResponseCrafter.HttpExceptions;

namespace ResponseCrafter.Demo.Hubs;

public class ChatHub : Hub<IChatClient>
{
   public override async Task OnConnectedAsync()
   {
      await Clients.All.ReceiveMessage($"{Context.ConnectionId} joined the chat");
      await base.OnConnectedAsync();
   }
   
   public async Task SendMessage(HubArgument<Message> hubArgument)
   {
      throw new BadRequestException("This is a test exception");
      await Clients.All.ReceiveMessage(hubArgument.Argument);
   }
}

public class Message
{
   public required string User { get; set; }
   public required string Content { get; set; }
}