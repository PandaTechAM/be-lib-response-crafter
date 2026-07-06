using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ResponseCrafter.HttpExceptions;

/// <summary>Exception that maps to an HTTP 409 Conflict response.</summary>
/// <param name="message">Error message describing the conflict.</param>
public class ConflictException(string message = ConflictException.DefaultMessage) : ApiException(409, message)
{
    private const string DefaultMessage =
        "the_request_could_not_be_completed_due_to_a_conflict_with_the_current_state_of_the_target_resource.";

    /// <summary>Throws a <see cref="ConflictException" /> when the value is null.</summary>
    public static void ThrowIfNull([NotNull] object? value, string exceptionMessage)
    {
        if (value is null)
        {
            throw new ConflictException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="ConflictException" /> when the value is null.</summary>
    public static void ThrowIfNull([NotNull] object? value)
    {
        if (value is null)
        {
            throw new ConflictException();
        }
    }

    /// <summary>Throws a <see cref="ConflictException" /> when the sequence is null or empty.</summary>
    public static void ThrowIfNullOrEmpty([NotNull] IEnumerable? value)
    {
        // ReSharper disable once GenericEnumeratorNotDisposed
        if (value is null || !value.GetEnumerator()
                .MoveNext())
        {
            throw new ConflictException();
        }
    }

    /// <summary>Throws a <see cref="ConflictException" /> when the sequence is null or empty.</summary>
    public static void ThrowIfNullOrEmpty([NotNull] IEnumerable? value, string exceptionMessage)
    {
        // ReSharper disable once GenericEnumeratorNotDisposed
        if (value is null || !value.GetEnumerator()
                .MoveNext())
        {
            throw new ConflictException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="ConflictException" /> when the string is null, empty, or whitespace.</summary>
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ConflictException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="ConflictException" /> when the string is null, empty, or whitespace.</summary>
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ConflictException();
        }
    }


    /// <summary>Throws a <see cref="ConflictException" /> when the condition is true.</summary>
    public static void ThrowIf(bool condition, string exceptionMessage)
    {
        if (condition)
        {
            throw new ConflictException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="ConflictException" /> when the condition is true.</summary>
    public static void ThrowIf(bool condition)
    {
        if (condition)
        {
            throw new ConflictException();
        }
    }

    /// <summary>Throws a <see cref="ConflictException" /> when the value is null or negative.</summary>
    public static void ThrowIfNullOrNegative([NotNull] decimal? value, string exceptionMessage)
    {
        if (value is < 0 or null)
        {
            throw new ConflictException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="ConflictException" /> when the value is null or negative.</summary>
    public static void ThrowIfNullOrNegative([NotNull] decimal? value)
    {
        if (value is < 0 or null)
        {
            throw new ConflictException();
        }
    }
}
