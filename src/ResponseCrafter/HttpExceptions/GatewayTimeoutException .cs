using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ResponseCrafter.HttpExceptions;

/// <summary>Exception that maps to an HTTP 504 Gateway Timeout response.</summary>
public class GatewayTimeoutException : ApiException
{
    private const string DefaultMessage = "the_request_timed_out_while_waiting_for_an_upstream_server.";

    /// <summary>Initializes a new <see cref="GatewayTimeoutException" /> with a message and optional field errors.</summary>
    public GatewayTimeoutException(string message = DefaultMessage, Dictionary<string, string>? errors = null)
        : base(504, message, errors)
    {
    }

    /// <summary>Throws a <see cref="GatewayTimeoutException" /> when the value is null.</summary>
    public static void ThrowIfNull([NotNull] object? value, string exceptionMessage,
        Dictionary<string, string>? errors = null)
    {
        if (value is null)
        {
            throw new GatewayTimeoutException(exceptionMessage, errors);
        }
    }

    /// <summary>Throws a <see cref="GatewayTimeoutException" /> when the sequence is null or empty.</summary>
    public static void ThrowIfNullOrEmpty([NotNull] IEnumerable? value, string exceptionMessage,
        Dictionary<string, string>? errors = null)
    {
        if (value is null || !value.GetEnumerator().MoveNext())
        {
            throw new GatewayTimeoutException(exceptionMessage, errors);
        }
    }

    /// <summary>Throws a <see cref="GatewayTimeoutException" /> when the string is null, empty, or whitespace.</summary>
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage,
        Dictionary<string, string>? errors = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new GatewayTimeoutException(exceptionMessage, errors);
        }
    }

    /// <summary>Throws a <see cref="GatewayTimeoutException" /> when the condition is true.</summary>
    public static void ThrowIf(bool condition, string exceptionMessage, Dictionary<string, string>? errors = null)
    {
        if (condition)
        {
            throw new GatewayTimeoutException(exceptionMessage, errors);
        }
    }

    /// <summary>Throws a <see cref="GatewayTimeoutException" /> when the value is null or negative.</summary>
    public static void ThrowIfNullOrNegative([NotNull] decimal? value, string exceptionMessage,
        Dictionary<string, string>? errors = null)
    {
        if (value is < 0 or null)
        {
            throw new GatewayTimeoutException(exceptionMessage, errors);
        }
    }
}
