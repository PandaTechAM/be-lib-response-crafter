using ResponseCrafter.Dtos;

namespace ResponseCrafter.StandardHttpExceptions;

public class InternalServerErrorException : ApiException
{
    private const string DefaultMessage = "an_internal_server_error_occurred.";

    public InternalServerErrorException(string message, List<ErrorDetail>? errors = null)
        : base(500, message, errors)
    {
    }

    public InternalServerErrorException(List<ErrorDetail> errors)
        : base(500, DefaultMessage, errors)
    {
    }
}