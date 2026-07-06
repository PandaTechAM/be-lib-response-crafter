using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ResponseCrafter.HttpExceptions;

/// <summary>Exception that maps to an HTTP 400 Bad Request response.</summary>
public class BadRequestException : ApiException
{
    private const string DefaultMessage = "the_request_was_invalid_or_cannot_be_otherwise_served.";

    /// <summary>Initializes a new <see cref="BadRequestException" /> with a message and optional field errors.</summary>
    public BadRequestException(string message, Dictionary<string, string>? errors = null)
        : base(400, message, errors)
    {
    }

    /// <summary>Initializes a new <see cref="BadRequestException" /> with field errors and the default message.</summary>
    public BadRequestException(Dictionary<string, string> errors)
        : base(400, DefaultMessage, errors)
    {
    }

    /// <summary>Throws a <see cref="BadRequestException" /> when the value is null.</summary>
    public static void ThrowIfNull([NotNull] object? value, string exceptionMessage)
    {
        if (value is null)
        {
            throw new BadRequestException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="BadRequestException" /> when the value is null.</summary>
    public static void ThrowIfNull([NotNull] object? value, string exceptionMessage,
        Dictionary<string, string>? errors = null)
    {
        if (value is null)
        {
            throw new BadRequestException(exceptionMessage, errors);
        }
    }

    /// <summary>Throws a <see cref="BadRequestException" /> when the sequence is null or empty.</summary>
    public static void ThrowIfNullOrEmpty([NotNull] IEnumerable? value, string exceptionMessage)
    {
        // ReSharper disable once GenericEnumeratorNotDisposed
        if (value is null || !value.GetEnumerator()
                .MoveNext())
        {
            throw new BadRequestException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="BadRequestException" /> when the sequence is null or empty.</summary>
    public static void ThrowIfNullOrEmpty([NotNull] IEnumerable? value, string exceptionMessage,
        Dictionary<string, string>? errors = null)
    {
        if (value is null || !value.GetEnumerator()
                .MoveNext())
        {
            throw new BadRequestException(exceptionMessage, errors);
        }
    }

    /// <summary>Throws a <see cref="BadRequestException" /> when the string is null, empty, or whitespace.</summary>
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BadRequestException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="BadRequestException" /> when the string is null, empty, or whitespace.</summary>
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage,
        Dictionary<string, string>? errors = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BadRequestException(exceptionMessage, errors);
        }
    }

    /// <summary>Throws a <see cref="BadRequestException" /> when the condition is true.</summary>
    public static void ThrowIf(bool condition, string exceptionMessage)
    {
        if (condition)
        {
            throw new BadRequestException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="BadRequestException" /> when the condition is true.</summary>
    public static void ThrowIf(bool condition, string exceptionMessage, Dictionary<string, string>? errors = null)
    {
        if (condition)
        {
            throw new BadRequestException(exceptionMessage, errors);
        }
    }

    /// <summary>Throws a <see cref="BadRequestException" /> when the value is null or negative.</summary>
    public static void ThrowIfNullOrNegative([NotNull] decimal? value, string exceptionMessage)
    {
        if (value is < 0 or null)
        {
            throw new BadRequestException(exceptionMessage);
        }
    }

    /// <summary>Throws a <see cref="BadRequestException" /> when the value is null or negative.</summary>
    public static void ThrowIfNullOrNegative([NotNull] decimal? value, string exceptionMessage,
        Dictionary<string, string>? errors = null)
    {
        if (value is < 0 or null)
        {
            throw new BadRequestException(exceptionMessage, errors);
        }
    }
}
