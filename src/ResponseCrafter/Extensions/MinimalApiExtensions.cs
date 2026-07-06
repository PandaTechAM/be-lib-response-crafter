using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ResponseCrafter.Dtos;

namespace ResponseCrafter.Extensions;

/// <summary>
///     Extension methods for Minimal API route builders to add ProblemDetails documentation.
/// </summary>
public static class MinimalApiExtensions
{
    /// <summary>
    ///     Adds ErrorResponse production for a specific status code.
    /// </summary>
    public static RouteHandlerBuilder ProducesErrorResponse(this RouteHandlerBuilder builder, int statusCode)
    {
        return builder.Produces<ErrorResponse>(statusCode);
    }

    /// <summary>
    ///     Adds ErrorResponse production for multiple status codes.
    /// </summary>
    public static RouteHandlerBuilder ProducesErrorResponse(this RouteHandlerBuilder builder, params int[] statusCodes)
    {
        foreach (var statusCode in statusCodes)
        {
            builder.Produces<ErrorResponse>(statusCode);
        }

        return builder;
    }

    /// <summary>
    ///     Adds 400 Bad Request ErrorResponse documentation.
    /// </summary>
    public static RouteHandlerBuilder ProducesBadRequest(this RouteHandlerBuilder builder)
    {
        return builder.Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    ///     Adds 409 Conflict ErrorResponse documentation.
    /// </summary>
    public static RouteHandlerBuilder ProducesConflict(this RouteHandlerBuilder builder)
    {
        return builder.Produces<ErrorResponse>(StatusCodes.Status409Conflict);
    }

    /// <summary>
    ///     Adds 403 Forbidden ErrorResponse documentation.
    /// </summary>
    public static RouteHandlerBuilder ProducesForbidden(this RouteHandlerBuilder builder)
    {
        return builder.Produces<ErrorResponse>(StatusCodes.Status403Forbidden);
    }

    /// <summary>
    ///     Adds 404 Not Found ErrorResponse documentation.
    /// </summary>
    public static RouteHandlerBuilder ProducesNotFound(this RouteHandlerBuilder builder)
    {
        return builder.Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }

    /// <summary>
    ///     Adds 402 Payment Required ErrorResponse documentation.
    /// </summary>
    public static RouteHandlerBuilder ProducesPaymentRequired(this RouteHandlerBuilder builder)
    {
        return builder.Produces<ErrorResponse>(StatusCodes.Status402PaymentRequired);
    }

    /// <summary>
    ///     Adds 503 Service Unavailable ErrorResponse documentation.
    /// </summary>
    public static RouteHandlerBuilder ProducesServiceUnavailable(this RouteHandlerBuilder builder)
    {
        return builder.Produces<ErrorResponse>(StatusCodes.Status503ServiceUnavailable);
    }

    /// <summary>
    ///     Adds 429 Too Many Requests ErrorResponse documentation.
    /// </summary>
    public static RouteHandlerBuilder ProducesTooManyRequests(this RouteHandlerBuilder builder)
    {
        return builder.Produces<ErrorResponse>(StatusCodes.Status429TooManyRequests);
    }

    /// <summary>
    ///     Adds 401 Unauthorized ErrorResponse documentation.
    /// </summary>
    public static RouteHandlerBuilder ProducesUnauthorized(this RouteHandlerBuilder builder)
    {
        return builder.Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);
    }
}
