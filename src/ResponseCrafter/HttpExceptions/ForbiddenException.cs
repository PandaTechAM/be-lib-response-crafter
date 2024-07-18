using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ResponseCrafter.HttpExceptions;

public class ForbiddenException(string message = ForbiddenException.DefaultMessage) : ApiException(403, message)
{
    private const string DefaultMessage = "you_are_not_authorized_to_perform_this_action.";

    public static void ThrowIfNull([NotNull] object? value, string exceptionMessage)
    {
        if (value is null)
        {
            throw new ForbiddenException(exceptionMessage);
        }
    }

    public static void ThrowIfNull([NotNull] object? value)
    {
        if (value is null)
        {
            throw new ForbiddenException();
        }
    }
    
    public static void ThrowIfNullOrEmpty([NotNull] IEnumerable? value)
    {
       // ReSharper disable once GenericEnumeratorNotDisposed
       if (value is null || !value.GetEnumerator()
                                  .MoveNext())
       {
          throw new ForbiddenException();
       }
    }
   
    public static void ThrowIfNullOrEmpty([NotNull] IEnumerable? value, string exceptionMessage)
    {
       // ReSharper disable once GenericEnumeratorNotDisposed
       if (value is null || !value.GetEnumerator()
                                  .MoveNext())
       {
          throw new ForbiddenException(exceptionMessage);
       }
    }

    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ForbiddenException(exceptionMessage);
        }
    }
    
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ForbiddenException();
        }
    }

    public static void ThrowIf(bool condition, string exceptionMessage)
    {
        if (condition)
        {
            throw new ForbiddenException(exceptionMessage);
        }
    }
    
    public static void ThrowIf(bool condition)
    {
        if (condition)
        {
            throw new ForbiddenException();
        }
    }

    public static void ThrowIfNullOrNegative([NotNull] decimal? value, string exceptionMessage)
    {
        if (value is < 0 or null)
        {
            throw new ForbiddenException(exceptionMessage);
        }
    }
    
    public static void ThrowIfNullOrNegative([NotNull] decimal? value)
    {
        if (value is < 0 or null)
        {
            throw new ForbiddenException();
        }
    }
}