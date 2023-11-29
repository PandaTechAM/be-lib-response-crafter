namespace ResponseCrafter.StandardHttpExceptions;

public class ConflictException(string message = ConflictException.DefaultMessage) : ApiException(409, message)
{
    private const string DefaultMessage = "the_request_could_not_be_completed_due_to_a_conflict_with_the_current_state_of_the_target_resource.";
}