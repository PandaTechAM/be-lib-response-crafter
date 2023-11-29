namespace ResponseCrafter.Dtos;

public class ErrorResponse
{
    public string TraceId { get; set; } = null!;
    public string Instance { get; set; } = null!;
    public int StatusCode { get; set; }
    public string Type { get; set; } = null!;
    public Dictionary<string, string>? Errors { get; set; }
    public string Message { get; set; } = null!;
}