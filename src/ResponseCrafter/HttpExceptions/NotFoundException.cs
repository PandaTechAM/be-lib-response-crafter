using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ResponseCrafter.HttpExceptions;

/// <summary>Exception that maps to an HTTP 404 Not Found response.</summary>
/// <param name="message">Error message describing the missing resource.</param>
public class NotFoundException(string message = NotFoundException.DefaultMessage) : ApiException(404, message)
{
    private const string DefaultMessage = "the_requested_resource_was_not_found.";

    /// <summary>Throws a <see cref="NotFoundException" /> when the value is null.</summary>
    public static void ThrowIfNull([NotNull] object? value, string exceptionMessage)
    {
        if (value is null)
        {
            throw new NotFoundException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="NotFoundException" /> when the value is null.</summary>
    public static void ThrowIfNull([NotNull] object? value)
    {
        if (value is null)
        {
            throw new NotFoundException();
        }
    }

    /// <summary>Throws a <see cref="NotFoundException" /> when the sequence is null or empty.</summary>
    public static void ThrowIfNullOrEmpty([NotNull] IEnumerable? value)
    {
        // ReSharper disable once GenericEnumeratorNotDisposed
        if (value is null || !value.GetEnumerator()
                .MoveNext())
        {
            throw new NotFoundException();
        }
    }

    /// <summary>Throws a <see cref="NotFoundException" /> when the sequence is null or empty.</summary>
    public static void ThrowIfNullOrEmpty([NotNull] IEnumerable? value, string exceptionMessage)
    {
        // ReSharper disable once GenericEnumeratorNotDisposed
        if (value is null || !value.GetEnumerator()
                .MoveNext())
        {
            throw new NotFoundException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="NotFoundException" /> when the string is null, empty, or whitespace.</summary>
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new NotFoundException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="NotFoundException" /> when the string is null, empty, or whitespace.</summary>
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new NotFoundException();
        }
    }


    /// <summary>Throws a <see cref="NotFoundException" /> when the condition is true.</summary>
    public static void ThrowIf(bool condition, string exceptionMessage)
    {
        if (condition)
        {
            throw new NotFoundException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="NotFoundException" /> when the condition is true.</summary>
    public static void ThrowIf(bool condition)
    {
        if (condition)
        {
            throw new NotFoundException();
        }
    }

    /// <summary>Throws a <see cref="NotFoundException" /> when the value is null or negative.</summary>
    public static void ThrowIfNullOrNegative([NotNull] decimal? value, string exceptionMessage)
    {
        if (value is < 0 or null)
        {
            throw new NotFoundException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="NotFoundException" /> when the value is null or negative.</summary>
    public static void ThrowIfNullOrNegative([NotNull] decimal? value)
    {
        if (value is < 0 or null)
        {
            throw new NotFoundException();
        }
    }
}
