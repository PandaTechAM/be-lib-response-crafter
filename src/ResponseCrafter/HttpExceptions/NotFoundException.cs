namespace ResponseCrafter.HttpExceptions;

public class NotFoundException(string message = NotFoundException.DefaultMessage) : ApiException(404, message)
{
    private const string DefaultMessage = "the_requested_resource_was_not_found.";
    
    public static void ThrowIfNullOrWhiteSpace(string? value, string? nameOfValue = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new NotFoundException($"{nameOfValue ?? "the_requested_resource"}_was_not_found.");
        }
    }
    
    public static void ThrowIfNull(object? value, string? nameOfValue = null)
    {
        if (value is null)
        {
            throw new NotFoundException($"{nameOfValue ?? "the_requested_resource"}_was_not_found.");
        }
    }
    
    public static void ThrowIfNullOrWhiteSpace(List<string?>? values, string? nameOfValue = null)
    {
        if (values is null || values.Count == 0 || values.Any(string.IsNullOrWhiteSpace))
        {
            throw new BadRequestException($"{nameOfValue ?? "the_requested_resource"}_was_not_found.");
        }
    }
}
