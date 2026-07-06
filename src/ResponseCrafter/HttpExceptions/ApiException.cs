namespace ResponseCrafter.HttpExceptions;

/// <summary>Base type for all HTTP status exceptions handled by ResponseCrafter.</summary>
/// <param name="statusCode">HTTP status code returned to the client.</param>
/// <param name="message">Error message describing the failure.</param>
/// <param name="errors">Optional field-level validation errors keyed by field name.</param>
public abstract class ApiException(
    int statusCode,
    string message,
    Dictionary<string, string>? errors = null) : Exception(message)
{
    /// <summary>HTTP status code returned to the client.</summary>
    public int StatusCode { get; private set; } = statusCode;

    /// <summary>Optional field-level validation errors keyed by field name.</summary>
    public Dictionary<string, string>? Errors { get; internal set; } = errors;
}
