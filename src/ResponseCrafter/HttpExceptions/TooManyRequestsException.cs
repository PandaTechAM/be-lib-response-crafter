namespace ResponseCrafter.HttpExceptions;

/// <summary>Exception that maps to an HTTP 429 Too Many Requests response.</summary>
public class TooManyRequestsException : ApiException
{
    private const string DefaultMessage =
        "you_have_sent_too_many_requests_in_a_given_amount_of_time._please_try_again_later.";

    /// <summary>Initializes a new <see cref="TooManyRequestsException" /> with a message.</summary>
    public TooManyRequestsException(string message = DefaultMessage)
        : base(429, message)
    {
    }

    /// <summary>Initializes a new <see cref="TooManyRequestsException" /> with a message and optional field errors.</summary>
    public TooManyRequestsException(string message, Dictionary<string, string>? errors = null)
        : base(429, message, errors)
    {
    }
}
