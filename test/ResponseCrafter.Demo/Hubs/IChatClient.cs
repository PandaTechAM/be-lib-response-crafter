namespace ResponseCrafter.Demo.Hubs;

public interface IChatClient
{
   Task ReceiveMessage(string message);
   Task ReceiveMessage(Message message);
}