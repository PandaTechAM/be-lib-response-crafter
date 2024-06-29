using System.Diagnostics.CodeAnalysis;

namespace ResponseCrafter.HttpExceptions;

public class ConflictException(string message = ConflictException.DefaultMessage) : ApiException(409, message)
{
    private const string DefaultMessage =
        "the_request_could_not_be_completed_due_to_a_conflict_with_the_current_state_of_the_target_resource.";

    public static void ThrowIfNull([NotNull] object? value, string exceptionMessage)
    {
        if (value is null)
        {
            throw new ConflictException(exceptionMessage);
        }
    }

    public static void ThrowIfNull([NotNull] object? value)
    {
        if (value is null)
        {
            throw new ConflictException();
        }
    }

    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ConflictException(exceptionMessage);
        }
    }

    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ConflictException();
        }
    }

    public static void ThrowIfNullOrWhiteSpace([NotNull] List<string?>? values, string exceptionMessage)
    {
        if (values is null || values.Count == 0 || values.Any(string.IsNullOrWhiteSpace))
        {
            throw new ConflictException(exceptionMessage);
        }
    }

    public static void ThrowIfNullOrWhiteSpace([NotNull] List<string?>? values)
    {
        if (values is null || values.Count == 0 || values.Any(string.IsNullOrWhiteSpace))
        {
            throw new ConflictException();
        }
    }

    public static void ThrowIf(bool condition, string exceptionMessage)
    {
        if (condition)
        {
            throw new ConflictException(exceptionMessage);
        }
    }

    public static void ThrowIf(bool condition)
    {
        if (condition)
        {
            throw new ConflictException();
        }
    }

    public static void ThrowIfNullOrNegative([NotNull] decimal? value, string exceptionMessage)
    {
        if (value is < 0 or null)
        {
            throw new ConflictException(exceptionMessage);
        }
    }

    public static void ThrowIfNullOrNegative([NotNull] decimal? value)
    {
        if (value is < 0 or null)
        {
            throw new ConflictException();
        }
    }
}