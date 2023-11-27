using ResponseCrafter.Dtos;

namespace ResponseCrafter;

public abstract class ApiException(int statusCode, string message,
    List<ErrorDetail>? errors = null): Exception(message)
{
    public int StatusCode { get; private set; } = statusCode;
    public List<ErrorDetail>? Errors { get; private set; } = errors;
}