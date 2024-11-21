namespace ResponseCrafter.HttpExceptions;

public class PaymentRequiredException(string message = PaymentRequiredException.DefaultMessage)
   : ApiException(402, message)
{
   private const string DefaultMessage = "payment_is_required_to_perform_this_action.";
}