namespace ResponseCrafter.StandardHttpExceptions;

public class InternalServerErrorException : ApiException
{
    private const string DefaultMessage = "an_internal_server_error_occurred.";

    public InternalServerErrorException(string message, Dictionary<string, string>? errors = null)
        : base(500, message, errors)
    {
    }

    public InternalServerErrorException(Dictionary<string, string> errors)
        : base(500, DefaultMessage, errors)
    {
    }
}