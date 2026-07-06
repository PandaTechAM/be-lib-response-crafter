namespace ResponseCrafter.HttpExceptions;

/// <summary>Exception that maps to an HTTP 469 response signalling a required password change.</summary>
/// <param name="message">Error message describing the password-change requirement.</param>
public class ForceToChangePasswordException(
    string message = ForceToChangePasswordException.DefaultMessage)
    : ApiException(469, message)
{
    private const string DefaultMessage = "password_change_required_to_proceed.";
}
