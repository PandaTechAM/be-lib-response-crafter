# Pandatech.ResponseCrafter

Exception-based error handling for ASP.NET Core 8+ with RFC 9457-compliant ProblemDetails, structured logging, and
SignalR support.

> **Note:** If you prefer the **Result pattern** over exception-based error handling, consider [**ResultCrafter
**](https://www.nuget.org/packages?q=ResultCrafter) by Haik Asatryan. ResultCrafter provides a more functional approach
> with explicit error handling through Result types. ResponseCrafter takes a different path—using typed exceptions with
> automatic ProblemDetails mapping—suitable for teams that prefer traditional exception flows.

## Installation

```bash
dotnet add package Pandatech.ResponseCrafter
```

## Setup

```csharp
using ResponseCrafter.Enums;
using ResponseCrafter.Extensions;

builder.AddResponseCrafter(NamingConvention.ToSnakeCase);

// Optional: SignalR support
builder.Services.AddSignalR(o => o.AddFilter<SignalRExceptionFilter>());

var app = builder.Build();

app.UseResponseCrafter();
```

## Quick Start

### Throw Typed Exceptions

```csharp
using ResponseCrafter.HttpExceptions;

app.MapPost("/users", (CreateUserRequest req) =>
{
    // Validation
    BadRequestException.ThrowIfNullOrWhiteSpace(req.Email, "email_required");
    
    // Not found
    var user = db.Users.Find(req.Id);
    NotFoundException.ThrowIfNull(user, "user_not_found");
    
    // Conflict
    if (db.Users.Any(u => u.Email == req.Email))
        throw new ConflictException("email_already_exists");
    
    // With field errors
    throw new BadRequestException("invalid_payload", new Dictionary<string, string>
    {
        ["email"] = "invalid_email_format",
        ["age"] = "must_be_18_or_older"
    });
});
```

### Response Format

All errors return RFC 9457 ProblemDetails:

```json
{
    "requestId": "0HN8K2MJ7F4QP:00000001",
    "traceId": "00-abc123...",
    "instance": "/api/users",
    "statusCode": 400,
    "type": "BadRequestException",
    "message": "invalid_payload",
    "errors": {
        "email": "invalid_email_format",
        "age": "must_be_18_or_older"
    }
}
```

## Available Exceptions

| Exception                        | Status | Static Helpers                 |
|----------------------------------|--------|--------------------------------|
| `BadRequestException`            | 400    | ✅ ThrowIf*, ThrowIfNull*, etc. |
| `UnauthorizedException`          | 401    | ✅                              |
| `PaymentRequiredException`       | 402    | ❌                              |
| `ForbiddenException`             | 403    | ✅                              |
| `NotFoundException`              | 404    | ✅                              |
| `ConflictException`              | 409    | ✅                              |
| `TooManyRequestsException`       | 429    | ❌                              |
| `ForceToChangePasswordException` | 469    | ❌                              |
| `InternalServerErrorException`   | 500    | ✅                              |
| `ServiceUnavailableException`    | 503    | ✅                              |
| `GatewayTimeoutException`        | 504    | ✅                              |

## Static Helper Methods

All exceptions with static helpers provide:

```csharp
// Null checks
NotFoundException.ThrowIfNull(user);
NotFoundException.ThrowIfNull(user, "custom_message");

// String checks
BadRequestException.ThrowIfNullOrWhiteSpace(input, "input_required");

// Collection checks
NotFoundException.ThrowIfNullOrEmpty(list, "no_results_found");

// Conditional
ForbiddenException.ThrowIf(condition, "access_denied");

// Numeric checks
BadRequestException.ThrowIfNullOrNegative(amount, "invalid_amount");
```

## Naming Conventions

ResponseCrafter automatically converts error messages and field names to your preferred case:

```csharp
builder.AddResponseCrafter(NamingConvention.ToSnakeCase);
```

**Available conventions:**

- `Default` - No transformation
- `ToSnakeCase` - `user_not_found`
- `ToCamelCase` - `userNotFound`
- `ToPascalCase` - `UserNotFound`
- `ToKebabCase` - `user-not-found`
- `ToTitleCase` - `User Not Found`
- `ToHumanCase` - `User not found`
- `ToUpperSnakeCase` - `USER_NOT_FOUND`

## SignalR Support

Handle exceptions in SignalR hubs with automatic error broadcasting:

```csharp
using ResponseCrafter.ExceptionHandlers.SignalR;

// Hub method
public class ChatHub : Hub<IChatClient>
{
    public Task SendMessage(Message request)
    {
        BadRequestException.ThrowIfNullOrWhiteSpace(request.Content, "message_empty");
        
        // Process message...
    }
}

// Request must implement IHubArgument
public class Message : IHubArgument
{
    public required string Content { get; set; }
    public required string InvocationId { get; set; }
}
```

**Client receives error via `ReceiveError` event:**

```json
{
    "traceId": "00-abc123...",
    "invocationId": "unique-call-id",
    "instance": "SendMessage",
    "statusCode": 400,
    "message": "message_empty",
    "errors": null
}
```

## Automatic Exception Mapping

ResponseCrafter automatically handles:

- **DbUpdateConcurrencyException** → 409 Conflict
- **FluentImporter exceptions** → 400 Bad Request
- **Gridify exceptions** → 400 Bad Request
- **BadHttpRequestException** (malformed JSON) → 400 with parse details
- **Unhandled exceptions** → 500 Internal Server Error

## Error Visibility

Control error detail exposure via configuration:

```json
{
    "ResponseCrafterVisibility": "Public"
}
```

- **Public** (default): 5xx errors show generic message, 4xx show actual errors
- **Private**: All errors show full details (dev/staging only)

## OpenAPI Integration

```csharp
app.MapPost("/users", (CreateUserRequest req) =>
{
    // ... handler logic
})
.ProducesBadRequest()
.ProducesConflict()
.ProducesNotFound();
```

**Available extension methods:**

- `ProducesBadRequest()` - 400
- `ProducesUnauthorized()` - 401
- `ProducesForbidden()` - 403
- `ProducesNotFound()` - 404
- `ProducesConflict()` - 409
- `ProducesTooManyRequests()` - 429
- `ProducesServiceUnavailable()` - 503
- `ProducesPaymentRequired()` - 402
- `ProducesErrorResponse(statusCode)` - Custom status
- `ProducesErrorResponse(params int[])` - Multiple statuses

## Advanced Usage

### Custom Error Dictionaries

```csharp
var errors = new Dictionary<string, string>
{
    ["field1"] = "error_message_1",
    ["field2"] = "error_message_2"
};

throw new BadRequestException("validation_failed", errors);
// or
throw new BadRequestException(errors); // Uses default message
```

### Conditional Throwing

```csharp
BadRequestException.ThrowIf(age < 18, "must_be_adult");
ForbiddenException.ThrowIf(!user.IsAdmin, "admin_only");
```

## Comparison with ResultCrafter

| Feature          | ResponseCrafter        | ResultCrafter                     |
|------------------|------------------------|-----------------------------------|
| **Approach**     | Exception-based        | Result-based (functional)         |
| **Control flow** | try/catch              | Explicit Result<T>                |
| **Performance**  | Exception overhead     | No exceptions                     |
| **API clarity**  | Implicit errors        | Explicit in signatures            |
| **Best for**     | Traditional .NET teams | Teams preferring functional style |

**Use ResponseCrafter if:**

- Your team is comfortable with exceptions
- You have existing exception-based code
- You prefer implicit error handling

**Use ResultCrafter if:**

- You want explicit error handling in method signatures
- You prefer functional programming patterns
- Performance is critical (no exception overhead)

## Configuration Reference

**appsettings.json:**

```json
{
    "ResponseCrafterVisibility": "Public"
}
```

**Program.cs:**

```csharp
builder.AddResponseCrafter(NamingConvention.ToSnakeCase);
app.UseResponseCrafter();
```

## License

MIT