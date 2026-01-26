using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ResponseCrafter.Dtos;

namespace ResponseCrafter.Extensions;

public static class MinimalApiExtensions
{
   extension(RouteHandlerBuilder builder)
   {
      public RouteHandlerBuilder ProducesErrorResponse(int statusCode)
      {
         return builder.Produces<ErrorResponse>(statusCode);
      }

      public RouteHandlerBuilder ProducesErrorResponse(params int[] statusCodes)
      {
         foreach (var statusCode in statusCodes)
         {
            builder.Produces<ErrorResponse>(statusCode);
         }

         return builder;
      }

      public RouteHandlerBuilder ProducesBadRequest()
      {
         return builder.Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
      }

      public RouteHandlerBuilder ProducesConflict()
      {
         return builder.Produces<ErrorResponse>(StatusCodes.Status409Conflict);
      }

      public RouteHandlerBuilder ProducesForbidden()
      {
         return builder.Produces<ErrorResponse>(StatusCodes.Status403Forbidden);
      }

      public RouteHandlerBuilder ProducesNotFound()
      {
         return builder.Produces<ErrorResponse>(StatusCodes.Status404NotFound);
      }

      public RouteHandlerBuilder ProducesPaymentRequired()
      {
         return builder.Produces<ErrorResponse>(StatusCodes.Status402PaymentRequired);
      }

      public RouteHandlerBuilder ProducesServiceUnavailable()
      {
         return builder.Produces<ErrorResponse>(StatusCodes.Status503ServiceUnavailable);
      }

      public RouteHandlerBuilder ProducesTooManyRequests()
      {
         return builder.Produces<ErrorResponse>(StatusCodes.Status429TooManyRequests);
      }

      public RouteHandlerBuilder ProducesUnauthorized()
      {
         return builder.Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);
      }
   }
}