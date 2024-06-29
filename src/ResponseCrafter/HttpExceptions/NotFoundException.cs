using System.Diagnostics.CodeAnalysis;

namespace ResponseCrafter.HttpExceptions;

public class NotFoundException(string message = NotFoundException.DefaultMessage) : ApiException(404, message)
{
    private const string DefaultMessage = "the_requested_resource_was_not_found.";

    public static void ThrowIfNull([NotNull] object? value, string exceptionMessage)
    {
        if (value is null)
        {
            throw new NotFoundException(exceptionMessage);
        }
    }

    public static void ThrowIfNull([NotNull] object? value)
    {
        if (value is null)
        {
            throw new NotFoundException();
        }
    }

    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new NotFoundException(exceptionMessage);
        }
    }
    
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new NotFoundException();
        }
    }

    public static void ThrowIfNullOrWhiteSpace([NotNull] List<string?>? values, string exceptionMessage)
    {
        if (values is null || values.Count == 0 || values.Any(string.IsNullOrWhiteSpace))
        {
            throw new NotFoundException(exceptionMessage);
        }
    }
    
    public static void ThrowIfNullOrWhiteSpace([NotNull] List<string?>? values)
    {
        if (values is null || values.Count == 0 || values.Any(string.IsNullOrWhiteSpace))
        {
            throw new NotFoundException();
        }
    }

    public static void ThrowIf(bool condition, string exceptionMessage)
    {
        if (condition)
        {
            throw new NotFoundException(exceptionMessage);
        }
    }
    
    public static void ThrowIf(bool condition)
    {
        if (condition)
        {
            throw new NotFoundException();
        }
    }

    public static void ThrowIfNullOrNegative([NotNull] decimal? value, string exceptionMessage)
    {
        if (value is < 0 or null)
        {
            throw new NotFoundException(exceptionMessage);
        }
    }
    
    public static void ThrowIfNullOrNegative([NotNull] decimal? value)
    {
        if (value is < 0 or null)
        {
            throw new NotFoundException();
        }
    }
}