using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ResponseCrafter.HttpExceptions;

/// <summary>Exception that maps to an HTTP 500 Internal Server Error response.</summary>
public class InternalServerErrorException : ApiException
{
    private const string DefaultMessage = "an_internal_server_error_occurred.";

    /// <summary>Initializes a new <see cref="InternalServerErrorException" /> with a message and optional field errors.</summary>
    public InternalServerErrorException(string message, Dictionary<string, string>? errors = null)
        : base(500, message, errors)
    {
    }

    /// <summary>Initializes a new <see cref="InternalServerErrorException" /> with field errors and the default message.</summary>
    public InternalServerErrorException(Dictionary<string, string> errors)
        : base(500, DefaultMessage, errors)
    {
    }

    /// <summary>Throws an <see cref="InternalServerErrorException" /> when the value is null.</summary>
    public static void ThrowIfNull([NotNull] object? value, string exceptionMessage)
    {
        if (value is null)
        {
            throw new InternalServerErrorException(exceptionMessage);
        }
    }

    /// <summary>Throws an <see cref="InternalServerErrorException" /> when the value is null.</summary>
    public static void ThrowIfNull([NotNull] object? value, string exceptionMessage,
        Dictionary<string, string>? errors = null)
    {
        if (value is null)
        {
            throw new InternalServerErrorException(exceptionMessage, errors);
        }
    }

    /// <summary>Throws an <see cref="InternalServerErrorException" /> when the sequence is null or empty.</summary>
    public static void ThrowIfNullOrEmpty([NotNull] IEnumerable? value, string exceptionMessage)
    {
        // ReSharper disable once GenericEnumeratorNotDisposed
        if (value is null || !value.GetEnumerator()
                .MoveNext())
        {
            throw new InternalServerErrorException(exceptionMessage);
        }
    }

    /// <summary>Throws an <see cref="InternalServerErrorException" /> when the sequence is null or empty.</summary>
    public static void ThrowIfNullOrEmpty([NotNull] IEnumerable? value, string exceptionMessage,
        Dictionary<string, string>? errors = null)
    {
        if (value is null || !value.GetEnumerator()
                .MoveNext())
        {
            throw new InternalServerErrorException(exceptionMessage, errors);
        }
    }

    /// <summary>Throws an <see cref="InternalServerErrorException" /> when the string is null, empty, or whitespace.</summary>
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InternalServerErrorException(exceptionMessage);
        }
    }

    /// <summary>Throws an <see cref="InternalServerErrorException" /> when the string is null, empty, or whitespace.</summary>
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage,
        Dictionary<string, string>? errors = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InternalServerErrorException(exceptionMessage, errors);
        }
    }

    /// <summary>Throws an <see cref="InternalServerErrorException" /> when the condition is true.</summary>
    public static void ThrowIf(bool condition, string exceptionMessage)
    {
        if (condition)
        {
            throw new InternalServerErrorException(exceptionMessage);
        }
    }

    /// <summary>Throws an <see cref="InternalServerErrorException" /> when the condition is true.</summary>
    public static void ThrowIf(bool condition, string exceptionMessage, Dictionary<string, string>? errors = null)
    {
        if (condition)
        {
            throw new InternalServerErrorException(exceptionMessage, errors);
        }
    }

    /// <summary>Throws an <see cref="InternalServerErrorException" /> when the value is null or negative.</summary>
    public static void ThrowIfNullOrNegative([NotNull] decimal? value, string exceptionMessage)
    {
        if (value is < 0 or null)
        {
            throw new InternalServerErrorException(exceptionMessage);
        }
    }

    /// <summary>Throws an <see cref="InternalServerErrorException" /> when the value is null or negative.</summary>
    public static void ThrowIfNullOrNegative([NotNull] decimal? value, string exceptionMessage,
        Dictionary<string, string>? errors = null)
    {
        if (value is < 0 or null)
        {
            throw new InternalServerErrorException(exceptionMessage, errors);
        }
    }
}
