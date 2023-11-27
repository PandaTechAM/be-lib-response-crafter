namespace ResponseCrafter.StandardHttpExceptions;

public class UnauthorizedException() : ApiException(401, DefaultMessage)
{
    private const string DefaultMessage = "your_token_is_invalid_or_expired.";
}
