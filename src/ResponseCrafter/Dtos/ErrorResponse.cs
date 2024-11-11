namespace ResponseCrafter.Dtos;

public class ErrorResponse
{
   public required string RequestId { get; set; }
   public required string TraceId { get; set; }
   public required string Instance { get; set; }
   public int StatusCode { get; init; }
   public required string Type { get; set; }
   public Dictionary<string, string>? Errors { get; set; }
   public required string Message { get; set; }
}