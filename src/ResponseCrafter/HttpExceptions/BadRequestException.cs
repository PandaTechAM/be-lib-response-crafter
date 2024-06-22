namespace ResponseCrafter.HttpExceptions;

public class BadRequestException : ApiException
{
    private const string DefaultMessage = "the_request_was_invalid_or_cannot_be_otherwise_served.";

    public BadRequestException(string message, Dictionary<string, string>? errors = null)
        : base(400, message, errors)
    {
    }

    public BadRequestException(Dictionary<string, string> errors)
        : base(400, DefaultMessage, errors)
    {
    }

    public static void ThrowIfNullOrNegative(decimal? value, string? nameOfValue = null)
    {
        if (value is < 0 or null)
        {
            throw new BadRequestException($"{nameOfValue ?? "value"}_cannot_be_negative");
        }
    }

    public static void ThrowIfNullOrWhiteSpace(string? value, string? nameOfValue = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BadRequestException($"{nameOfValue ?? "value"}_cannot_be_null_or_whitespace");
        }
    }

    public static void ThrowIfNullOrWhiteSpace(List<string?>? values, string? nameOfValue = null)
    {
        if (values is null || values.Count == 0 || values.Any(string.IsNullOrWhiteSpace))
        {
            throw new BadRequestException($"{nameOfValue ?? "value"}_cannot_contain_null_or_whitespace");
        }
    }

    public static void ThrowIfNull(object? value, string? nameOfValue = null)
    {
        if (value is null)
        {
            throw new BadRequestException($"{nameOfValue ?? "value"}_cannot_be_null");
        }
    }
}