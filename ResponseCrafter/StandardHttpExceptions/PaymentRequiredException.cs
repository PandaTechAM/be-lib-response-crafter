namespace ResponseCrafter.StandardHttpExceptions;

public class PaymentRequiredException() : ApiException(402, DefaultMessage)
{
    private const string DefaultMessage = "payment_is_required_to_perform_this_action.";
}
