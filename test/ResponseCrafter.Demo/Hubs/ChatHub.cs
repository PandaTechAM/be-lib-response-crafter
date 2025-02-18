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
   
   public async Task SendMessage(Message hubArgument)
   {
      throw new BadRequestException("This is a test exception");
      await Clients.All.ReceiveMessage(hubArgument.User);
   }
}

public class Message : IHubArgument
{
   public required string User { get; set; }
   public required string Content { get; set; }
   public required string InvocationId { get; set; }
}