using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ResponseCrafter.HttpExceptions;

/// <summary>Exception that maps to an HTTP 503 Service Unavailable response.</summary>
/// <param name="message">Error message describing the unavailability.</param>
public class ServiceUnavailableException(string message = ServiceUnavailableException.DefaultMessage)
    : ApiException(503, message)
{
    private const string DefaultMessage = "the_server_is_currently_unavailable._please_try_again_later.";

    /// <summary>Throws a <see cref="ServiceUnavailableException" /> when the value is null.</summary>
    public static void ThrowIfNull([NotNull] object? value, string exceptionMessage)
    {
        if (value is null)
        {
            throw new ServiceUnavailableException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="ServiceUnavailableException" /> when the sequence is null or empty.</summary>
    public static void ThrowIfNullOrEmpty([NotNull] IEnumerable? value, string exceptionMessage)
    {
        // ReSharper disable once GenericEnumeratorNotDisposed
        if (value is null || !value.GetEnumerator()
                .MoveNext())
        {
            throw new ServiceUnavailableException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="ServiceUnavailableException" /> when the string is null, empty, or whitespace.</summary>
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ServiceUnavailableException(exceptionMessage);
        }
    }


    /// <summary>Throws a <see cref="ServiceUnavailableException" /> when the condition is true.</summary>
    public static void ThrowIf(bool condition, string exceptionMessage)
    {
        if (condition)
        {
            throw new ServiceUnavailableException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="ServiceUnavailableException" /> when the value is null or negative.</summary>
    public static void ThrowIfNullOrNegative([NotNull] decimal? value, string exceptionMessage)
    {
        if (value is < 0 or null)
        {
            throw new ServiceUnavailableException(exceptionMessage);
        }
    }
}
