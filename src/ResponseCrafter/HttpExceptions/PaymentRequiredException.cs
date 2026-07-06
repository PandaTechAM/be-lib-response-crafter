namespace ResponseCrafter.HttpExceptions;

/// <summary>Exception that maps to an HTTP 402 Payment Required response.</summary>
/// <param name="message">Error message describing the payment requirement.</param>
public class PaymentRequiredException(string message = PaymentRequiredException.DefaultMessage)
    : ApiException(402, message)
{
    private const string DefaultMessage = "payment_is_required_to_perform_this_action.";
}
