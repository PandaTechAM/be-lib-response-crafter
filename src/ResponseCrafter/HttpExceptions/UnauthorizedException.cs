namespace ResponseCrafter.HttpExceptions;

public class UnauthorizedException(string message = UnauthorizedException.DefaultMessage) : ApiException(401, message)
{
    private const string DefaultMessage = "your_token_is_invalid_or_expired.";
}
