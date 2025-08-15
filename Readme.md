# Pandatech.ResponseCrafter

A lightweight exception handling and logging package for ASP.NET Core (Minimal APIs with first-class **SignalR**
support.  
It standardizes server-side error handling and produces consistent, frontend-friendly error payloads with messages
suitable for localization.

---

## Features

- **Unified exception handling** for REST & SignalR (built on ASP.NET Core’s `IExceptionHandler` and SignalR
  `IHubFilter`).
- **Visibility modes** (`Public` / `Private`) to control how much detail is exposed in responses (e.g., hide 5xx details
  in Public).
- **Frontend-friendly messages** via configurable naming conventions (e.g., snake_case).
- **Predefined HTTP exceptions** covering common 4xx/5xx cases + helpers (`ThrowIf...`) to reduce boilerplate.
- **Consistent logging** with correlation IDs (`RequestId`, `TraceId`) and level split (4xx → warning, 5xx → error).
- **SignalR** error envelope with `invocation_id` echo and `ReceiveError` channel.

---

## Installation

Use either NuGet Package Manager or the CLI:

```bash
dotnet add package Pandatech.ResponseCrafter
# or
Install-Package Pandatech.ResponseCrafter
```

---

## Quick Start (Minimal API)

**program.cs**

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1) Register ResponseCrafter (optional naming convention)
builder.AddResponseCrafter(NamingConvention.ToSnakeCase);

// 2) Configure SignalR (optional). The filter applies package behavior to hubs.
builder.Services.AddSignalR(options => options.AddFilter<SignalRExceptionFilter>());

var app = builder.Build();

// 3) Use ResponseCrafter middleware/exception handler
app.UseResponseCrafter();

app.MapGet("/ping", () => "pong");

app.MapHub<ChatHub>("/hubs/chat");

app.Run();
```

**appsettings.json**

```json
{
    "ResponseCrafterVisibility": "Public"
}
```

- **Public:** 4xx → detailed as defined; 5xx → generic message (no sensitive details).
- **Private:** 4xx/5xx → expands with verbose diagnostics (where available).

> **Note:** By default, `ResponseCrafter` suppresses the duplicate framework error log from
`Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware`.
> If you prefer to keep that log, opt out:
> ```csharp
> builder.AddResponseCrafter(> NamingConvention.ToSnakeCase,> suppressExceptionHandlerMiddlewareLog: false);
> ```

```
---

## Supported HTTP Status Codes

| Code | Description                                       |
|:-----|:--------------------------------------------------|
| 200  | Request succeeded.                                |
| 202  | Request accepted (e.g. order enqueued).           |
| 400  | Invalid request parameters or duplicate requests. |
| 401  | Authentication failed.                            |
| 403  | Insufficient permissions.                         |
| 404  | Resource not found.                               |
| 409  | Conflict (e.g. concurrency violation).            |
| 429  | Too many requests.                                |
| 500  | Server encountered an unexpected error.           |
| 503  | Service unavailable.                              |

---

### REST (HTTP/JSON)

**Content type:** `application/json`

```json
{
    "RequestId": "0HMVFE0A284AM:00000001",
    "TraceId": "a55582ab204162e66e124b0378776ab7",
    "Instance": "POST - api.example.com:443/users/register",
    "StatusCode": 400,
    "Type": "BadRequestException",
    "Errors": {
        "email": "email_address_is_not_in_a_valid_format",
        "password": "password_must_be_at_least_8_characters_long"
    },
    "Message": "the_request_was_invalid_or_cannot_be_otherwise_served."
}
```

**Fields**

- **RequestId** – ASP.NET Core `HttpContext.TraceIdentifier`.
- **TraceId** – distributed trace id (W3C, e.g., `Activity.Current?.TraceId`).
- **Instance** – contextual info (e.g., `METHOD - host:port/path`).
- **StatusCode** – HTTP status code associated with the error.
- **Type** – short descriptor (CLR exception name for API exceptions; `"InternalServerError"` for 5xx in Public).
- **Errors** – `Dictionary<string, string>` of field-level messages.
- **Message** – human- or key-like description.

### SignalR (ReceiveError)

Errors are sent to the calling client on **`ReceiveError`**.

```json
{
    "TraceId": "a55582ab204162e66e124b0378776ab7",
    "InvocationId": "0HMVFE0A0HMVFE0A284AMHMV00HMVFE0A284AM0A284AM",
    "Instance": "SendMessage",
    "StatusCode": 400,
    "Errors": {
        "email": "email_address_is_not_in_a_valid_format",
        "password": "password_must_be_at_least_8_characters_long"
    },
    "Message": "the_request_was_invalid_or_cannot_be_otherwise_served."
}
```

**Caller contract**

- Include a non-empty **`InvocationId`** in hub calls (see `HubArgument<T>`).
- The same `InvocationId` is echoed in errors to correlate requests/responses.

---

## Naming Conventions

Use `NamingConvention` to transform messages (`Message` and `Errors` values).  
**Recommendation:** `ToSnakeCase` or `ToUpperSnakeCase`.

```csharp
public enum NamingConvention
{
Default = 0,
ToSnakeCase = 1,
ToPascalCase = 2,
ToCamelCase = 3,
ToKebabCase = 4,
ToTitleCase = 5,
ToHumanCase = 6,
ToUpperSnakeCase = 7
}
```

---

## Custom Exceptions

Extend the base `ApiException` (package type) to create business-specific errors that serialize consistently.

**Predefined exceptions:**

- `BadRequestException`
- `UnauthorizedException`
- `PaymentRequiredException`
- `ForbiddenException`
- `NotFoundException`
- `ConflictException`
- `TooManyRequestsException`
- `InternalServerErrorException`
- `ServiceUnavailableException`

**Example: create a custom domain exception**

```csharp
using ResponseCrafter.HttpExceptions;

public sealed class OrderLimitExceededException : ApiException
{
public OrderLimitExceededException(string? message = null)
: base(statusCode: 400, message ?? "order_limit_exceeded")
{
Errors = new() { ["limit"] = "maximum_daily_order_limit_reached" };
}
}
```

**Usage in an endpoint**

```csharp
app.MapPost("/orders", (CreateOrderRequest req) =>
{
if (req.Quantity > 100)
throw new OrderLimitExceededException();

// normal work...
return Results.Accepted();
});
```

---

## Helper Methods (ThrowIf…)

Use the built-in helper methods to reduce guard boilerplate.

```csharp
decimal? price = -10.5m;
// 400 Bad Request
BadRequestException.ThrowIfNullOrNegative(price, "price_is_negative");
// 500 Internal Server Error
InternalServerErrorException.ThrowIfNullOrNegative(price, "price_is_negative");

string? username = "   ";
// 400 Bad Request
BadRequestException.ThrowIfNullOrWhiteSpace(username, "please_provide_username");
// 404 Not Found
NotFoundException.ThrowIfNullOrWhiteSpace(username);
// 500 Internal Server Error
InternalServerErrorException.ThrowIfNullOrWhiteSpace(username, "username_required");

List<int> tags = [];
// 400 Bad Request
BadRequestException.ThrowIfNullOrEmpty(tags, "please_provide_tags");
// 404 Not Found
NotFoundException.ThrowIfNullOrEmpty(tags);
// 500 Internal Server Error
InternalServerErrorException.ThrowIfNullOrEmpty(tags, "tags_required");

object? user = null;
// 400 Bad Request
BadRequestException.ThrowIfNull(user, "please_provide_user");
// 404 Not Found
NotFoundException.ThrowIfNull(user, "user_not_found");
// 500 Internal Server Error
InternalServerErrorException.ThrowIfNull(user, "user_required");

bool userUnauthorized = false;
// 401 Unauthorized
UnauthorizedException.ThrowIf(userUnauthorized, "user_is_unauthorized");
// 500 Internal Server Error
InternalServerErrorException.ThrowIf(userUnauthorized, "authorization_check_failed");
```

---

## SignalR Integration

**Register the filter**

```csharp
builder.Services.AddSignalR(options => options.AddFilter<SignalRExceptionFilter>());
```

**Define the hub argument contract**

```csharp
public interface IHubArgument
{
    string InvocationId { get; set; }
}

public class HubArgument<T> : IHubArgument
{
    public required string InvocationId { get; set; }
    public required T Argument { get; set; }
}
```

**Hub method example**

```csharp
public class ChatHub : Hub
{
public async Task SendMessage(HubArgument<Message> hubArgument)
{
// Example: intentionally throwing to demonstrate an error path
throw new BadRequestException("invalid_message_format");

    // Normally:
    // await Clients.All.SendAsync("ReceiveMessage", hubArgument.Argument);

}
}

public class Message : IHubArgument
{
    public required string Message {get; set;}
    public required string InvocationId { get; set; } 
}
```

**Client expectation**

- On error, server sends **`ReceiveError`** to the **caller** with the structure shown above.
- `InvocationId` in the request is echoed back in the error.
- Multi-argument hub methods are supported as long as at least one argument implements `IHubArgument` (
  backward-compatible).

---

## Logging & Telemetry

- **4xx** → logged as **Warning**
- **5xx** → logged as **Error**
- Correlate using **RequestId** (REST) / **InvocationId** (SignalR) and **TraceId** across services.
- The package emits enough context for centralized log aggregation systems.

> **Tip:** You can create logging scopes around `TraceId`/`RequestId` in your app if desired.

---

## Built-in Mappings & Behavior

- `DbUpdateConcurrencyException` → **409 Conflict**
- `BadHttpRequestException` → **400 Bad Request** with a stable “invalid payload” style message
- `GridifyException` / `GridifyMapperException` → **400 Bad Request** with normalized messages

**Unhandled exceptions**

- **Public** → 5xx concealed behind a generic message + `Type = "InternalServerError"`.
- **Private** → verbose error details are returned to aid debugging.

---

## Configuration Summary

**Visibility**

```json
{
    "ResponseCrafterVisibility": "Public"
}
```

- Switch to `Private` for internal environments to expose verbose details (do not enable in production).

**Naming Convention (optional)**

- In `program.cs`, pass your preferred convention:

```csharp
builder.AddResponseCrafter(NamingConvention.ToSnakeCase);
```

### Exception handler log suppression (optional)

Enabled by default to avoid duplicate request-error logs from ASP.NET Core’s
`ExceptionHandlerMiddleware`. This does not affect hosted service logs or other `Microsoft.*` categories.

```csharp
// Keep the framework error log (opt out of suppression)
builder.AddResponseCrafter(
    namingConvention: NamingConvention.ToSnakeCase,
    suppressExceptionHandlerMiddlewareLog: false);

```

---

## Frontend Integration Guidance

- Treat `Message` and `Errors` values as **localization keys**.
- Use `StatusCode` for UX-level branching; avoid coupling to `Type` names.
- Show `RequestId` / `TraceId` in error overlays to speed up support.
- For SignalR, correlate using `InvocationId`.

---

## Versioning & Compatibility

- **No breaking changes** to existing fields without a major version bump.
- We may add **optional** fields in minor versions—clients should ignore unknown fields.
- The SignalR error event name remains **`ReceiveError`**.

---

## Limitations

- Designed for **.NET 8+**.

---

## License

MIT

---
