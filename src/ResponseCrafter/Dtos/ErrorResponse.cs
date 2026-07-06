using System.ComponentModel;

namespace ResponseCrafter.Dtos;

/// <summary>Standard structure for error responses returned to HTTP clients.</summary>
public class ErrorResponse
{
    /// <summary>Correlation id of the request that produced the error.</summary>
    [DefaultValue("0HN8FG4CDDD6R:00000003")]
    public required string RequestId { get; set; }

    /// <summary>Distributed trace id associated with the request.</summary>
    [DefaultValue("b224190af31b6eb950ce4b84c9d44fae")]
    public required string TraceId { get; set; }

    /// <summary>Request method and path that produced the error.</summary>
    [DefaultValue("POST - localhost/users")]
    public required string Instance { get; set; }

    /// <summary>HTTP status code of the response.</summary>
    [DefaultValue(400)]
    public int StatusCode { get; init; }

    /// <summary>Exception type name that produced the error.</summary>
    [DefaultValue("BadRequestException")]
    public required string Type { get; set; }

    /// <summary>Optional field-level validation errors keyed by field name.</summary>
    [DefaultValue(null)]
    public Dictionary<string, string>? Errors { get; set; }

    /// <summary>Human-readable error message.</summary>
    [DefaultValue("duplicate_user")]
    public required string Message { get; set; }
}
