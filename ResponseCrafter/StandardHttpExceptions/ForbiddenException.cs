namespace ResponseCrafter.StandardHttpExceptions;

public class ForbiddenException(string message = ForbiddenException.DefaultMessage) : ApiException(403, DefaultMessage)
{
    private const string DefaultMessage = "you_are_not_authorized_to_perform_this_action.";
}
