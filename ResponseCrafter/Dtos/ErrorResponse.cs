namespace ResponseCrafter.Dtos;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Type { get; set; } = null!;
    public string Message { get; set; } = null!;
    public List<ErrorDetail>? Errors { get; set; }
    public string TraceId { get; set; } = null!;
    public string Instance { get; set; } = null!;
}