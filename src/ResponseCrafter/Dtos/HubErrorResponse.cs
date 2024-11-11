namespace ResponseCrafter.Dtos;

/// <summary>
/// Represents a standard structure for error responses sent back to SignalR clients.
/// </summary>
public class HubErrorResponse
{
   public required string TraceId { get; set; }
   public required string InvocationId { get; set; }
   public required string Instance { get; set; }
   public int StatusCode { get; set; }
   public string Message { get; set; } = string.Empty;
   public Dictionary<string, string>? Errors { get; set; }
}