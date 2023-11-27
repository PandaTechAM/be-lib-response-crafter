using ResponseCrafter.Dtos;

namespace ResponseCrafter.StandardHttpExceptions;

public class BadRequestException : ApiException
{
    private const string DefaultMessage = "the_request_was_invalid_or_cannot_be_otherwise_served.";
    
    public BadRequestException(string message, List<ErrorDetail>? errors = null)
        : base(400, message, errors)
    {
    }
    public BadRequestException(List<ErrorDetail> errors)
        : base(400,DefaultMessage, errors)
    {
    }
}