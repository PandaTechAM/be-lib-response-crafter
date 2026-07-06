namespace ResponseCrafter.Dtos;

/// <summary>
///     Represents a standard structure for error responses sent back to SignalR clients.
/// </summary>
public class HubErrorResponse
{
    /// <summary>Distributed trace id associated with the invocation.</summary>
    public required string TraceId { get; set; }

    /// <summary>Client-supplied id correlating the error with the originating hub invocation.</summary>
    public required string InvocationId { get; set; }

    /// <summary>Hub method name that produced the error.</summary>
    public required string Instance { get; set; }

    /// <summary>HTTP-style status code describing the error.</summary>
    public int StatusCode { get; set; }

    /// <summary>Human-readable error message.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Optional field-level validation errors keyed by field name.</summary>
    public Dictionary<string, string>? Errors { get; set; }
}
