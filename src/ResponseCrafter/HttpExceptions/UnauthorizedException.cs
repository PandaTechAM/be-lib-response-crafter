﻿using System.Diagnostics.CodeAnalysis;

namespace ResponseCrafter.HttpExceptions;

public class UnauthorizedException(string message = UnauthorizedException.DefaultMessage) : ApiException(401, message)
{
    private const string DefaultMessage = "your_token_is_invalid_or_expired.";
    
    public static void ThrowIfNull([NotNull] object? value, string exceptionMessage)
    {
        if (value is null)
        {
            throw new UnauthorizedException(exceptionMessage);
        }
    }

    public static void ThrowIfNull([NotNull] object? value)
    {
        if (value is null)
        {
            throw new UnauthorizedException();
        }
    }

    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new UnauthorizedException(exceptionMessage);
        }
    }
    
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new UnauthorizedException();
        }
    }

    public static void ThrowIfNullOrWhiteSpace([NotNull] List<string?>? values, string exceptionMessage)
    {
        if (values is null || values.Count == 0 || values.Any(string.IsNullOrWhiteSpace))
        {
            throw new UnauthorizedException(exceptionMessage);
        }
    }
    
    public static void ThrowIfNullOrWhiteSpace([NotNull] List<string?>? values)
    {
        if (values is null || values.Count == 0 || values.Any(string.IsNullOrWhiteSpace))
        {
            throw new UnauthorizedException();
        }
    }

    public static void ThrowIf(bool condition, string exceptionMessage)
    {
        if (condition)
        {
            throw new UnauthorizedException(exceptionMessage);
        }
    }
    
    public static void ThrowIf(bool condition)
    {
        if (condition)
        {
            throw new UnauthorizedException();
        }
    }

    public static void ThrowIfNullOrNegative([NotNull] decimal? value, string exceptionMessage)
    {
        if (value is < 0 or null)
        {
            throw new UnauthorizedException(exceptionMessage);
        }
    }
    
    public static void ThrowIfNullOrNegative([NotNull] decimal? value)
    {
        if (value is < 0 or null)
        {
            throw new UnauthorizedException();
        }
    }
}
