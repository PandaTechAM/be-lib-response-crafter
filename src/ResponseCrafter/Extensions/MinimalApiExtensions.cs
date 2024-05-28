﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ResponseCrafter.Dtos;

namespace ResponseCrafter.Extensions;

public static class MinimalApiExtensions
{
    public static RouteHandlerBuilder ProducesErrorResponse(this RouteHandlerBuilder builder, int statusCode)
    {
        return builder.Produces<ErrorResponse>(statusCode);
    }
}