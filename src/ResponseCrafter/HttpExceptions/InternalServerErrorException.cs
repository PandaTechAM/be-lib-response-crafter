using System.Diagnostics.CodeAnalysis;

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
    
    public static void ThrowIfNull([NotNull] object? value, string exceptionMessage)
    {
        if (value is null)
        {
            throw new InternalServerErrorException(exceptionMessage);
        }
    }

    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InternalServerErrorException(exceptionMessage);
        }
    }

    public static void ThrowIfNullOrWhiteSpace([NotNull] List<string?>? values, string exceptionMessage)
    {
        if (values is null || values.Count == 0 || values.Any(string.IsNullOrWhiteSpace))
        {
            throw new InternalServerErrorException(exceptionMessage);
        }
    }

    public static void ThrowIf(bool condition, string exceptionMessage)
    {
        if (condition)
        {
            throw new InternalServerErrorException(exceptionMessage);
        }
    }

    public static void ThrowIfNullOrNegative([NotNull] decimal? value, string exceptionMessage)
    {
        if (value is < 0 or null)
        {
            throw new InternalServerErrorException(exceptionMessage);
        }
    }
}