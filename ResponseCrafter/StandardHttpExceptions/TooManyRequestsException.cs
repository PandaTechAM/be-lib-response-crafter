namespace ResponseCrafter.StandardHttpExceptions;

public class TooManyRequestsException : ApiException
{
    private const string DefaultMessage = "you_have_sent_too_many_requests_in_a_given_amount_of_time._please_try_again_later.";

    public TooManyRequestsException()
        : base(429, DefaultMessage)
    {
    }
}
