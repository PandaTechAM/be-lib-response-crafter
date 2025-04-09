namespace ResponseCrafter.HttpExceptions;

public class ForceToChangePasswordException(
   string message = ForceToChangePasswordException.DefaultMessage)
   : ApiException(469, message)
{
   private const string DefaultMessage = "password_change_required_to_proceed.";
}