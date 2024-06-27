using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ResponseCrafter.Dtos;

namespace ResponseCrafter.Extensions;

public static class MinimalApiExtensions
{
    public static RouteHandlerBuilder ProducesErrorResponse(this RouteHandlerBuilder builder, int statusCode)
    {
        return builder.Produces<ErrorResponse>(statusCode);
    }

    public static RouteHandlerBuilder ProducesErrorResponse(this RouteHandlerBuilder builder, params int[] statusCodes)
    {
        foreach (var statusCode in statusCodes)
        {
            builder.Produces<ErrorResponse>(statusCode);
        }

        return builder;
    }

    public static RouteHandlerBuilder ProducesBadRequest(this RouteHandlerBuilder builder)
    {
        return builder.Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }

    public static RouteHandlerBuilder ProducesConflict(this RouteHandlerBuilder builder)
    {
        return builder.Produces<ErrorResponse>(StatusCodes.Status409Conflict);
    }

    public static RouteHandlerBuilder ProducesForbidden(this RouteHandlerBuilder builder)
    {
        return builder.Produces<ErrorResponse>(StatusCodes.Status403Forbidden);
    }

    public static RouteHandlerBuilder ProducesNotFound(this RouteHandlerBuilder builder)
    {
        return builder.Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }

    public static RouteHandlerBuilder ProducesPaymentRequired(this RouteHandlerBuilder builder)
    {
        return builder.Produces<ErrorResponse>(StatusCodes.Status402PaymentRequired);
    }

    public static RouteHandlerBuilder ProducesServiceUnavailable(this RouteHandlerBuilder builder)
    {
        return builder.Produces<ErrorResponse>(StatusCodes.Status503ServiceUnavailable);
    }

    public static RouteHandlerBuilder ProducesTooManyRequests(this RouteHandlerBuilder builder)
    {
        return builder.Produces<ErrorResponse>(StatusCodes.Status429TooManyRequests);
    }

    public static RouteHandlerBuilder ProducesUnauthorized(this RouteHandlerBuilder builder)
    {
        return builder.Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);
    }
}