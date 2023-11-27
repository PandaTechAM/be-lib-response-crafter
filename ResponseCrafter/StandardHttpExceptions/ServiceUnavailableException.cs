namespace ResponseCrafter.StandardHttpExceptions;

public class ServiceUnavailableException() : ApiException(503, DefaultMessage)
{
    private const string DefaultMessage = "the_server_is_currently_unavailable._please_try_again_later.";
}