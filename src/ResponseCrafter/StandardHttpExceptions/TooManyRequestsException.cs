namespace ResponseCrafter.StandardHttpExceptions;

public class TooManyRequestsException(string message = TooManyRequestsException.DefaultMessage) : ApiException(429, message)
{
    private const string DefaultMessage = "you_have_sent_too_many_requests_in_a_given_amount_of_time._please_try_again_later.";
}
