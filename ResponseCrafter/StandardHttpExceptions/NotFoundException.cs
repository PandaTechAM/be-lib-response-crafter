namespace ResponseCrafter.StandardHttpExceptions;

public class NotFoundException(string message = NotFoundException.DefaultMessage) : ApiException(404, message)
{
    private const string DefaultMessage = "the_requested_resource_was_not_found.";
}
