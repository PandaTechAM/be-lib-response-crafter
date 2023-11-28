using ResponseCrafter.Dtos;

namespace ResponseCrafter;

public abstract class ApiException(int statusCode, string message,
    Dictionary<string, string>? errors = null): Exception(message)
{
    public int StatusCode { get; private set; } = statusCode;
    public Dictionary<string, string>? Errors { get; private set; } = errors;
}