namespace ResponseCrafter.HttpExceptions;

public class InternalServerErrorException : ApiException
{
    private const string DefaultMessage = "an_internal_server_error_occurred.";

    public InternalServerErrorException(string message, Dictionary<string, string>? errors = null)
        : base(500, message, errors)
    {
    }

    public InternalServerErrorException(Dictionary<string, string> errors)
        : base(500, DefaultMessage, errors)
    {
    }

    public static void ThrowIfNullOrNegative(decimal? value, string? nameOfValue = null)
    {
        if (value is < 0 or null)
        {
            throw new InternalServerErrorException($"{DefaultMessage}{nameOfValue ?? ""}");
        }
    }

    public static void ThrowIfNullOrWhiteSpace(string? value, string? nameOfValue = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InternalServerErrorException($"{DefaultMessage}{nameOfValue ?? ""}");
        }
    }

    public static void ThrowIfNullOrWhiteSpace(List<string?>? values, string? nameOfValue = null)
    {
        if (values is null || values.Count == 0 || values.Any(string.IsNullOrWhiteSpace))
        {
            throw new InternalServerErrorException($"{DefaultMessage}{nameOfValue ?? ""}");
        }
    }
}