namespace ResponseCrafter.HttpExceptions;

public class ForbiddenException(string message = ForbiddenException.DefaultMessage) : ApiException(403, message)
{
    private const string DefaultMessage = "you_are_not_authorized_to_perform_this_action.";
}
