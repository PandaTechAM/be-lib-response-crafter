# Pandatech.ResponseCrafter

## Introduction

**Pandatech.ResponseCrafter** is a comprehensive NuGet package for .NET 8+, specifically designed to enhance exception
handling and logging in ASP.NET Core applications, and now extended to support SignalR hubs. This package simplifies
managing standard and custom exceptions by crafting detailed error responses suitable for both development and
production environments. It inherits all RFC 9457 Problem Details for HTTP APIs and even extends it.

## Features

* **Custom Exception Handling:** Streamlines the process of managing both standard HTTP exceptions and custom exceptions
  for both REST APIs and SignalR.
* **Detailed Error Responses:** Generates verbose error messages, including stack traces for in-depth debugging in
  development environments.
* **Environment-Sensitive Logging:** Provides flexible logging and response behavior based on visibility
  settings (`Public` or `Private`):
    - **Private:** All exceptions are sent to the client as defined, and 4xx errors are logged as warnings while 5xx
      errors are logged as errors.
    - **Public:** 4xx exceptions are sent to the client as defined, while 5xx errors are concealed with a generic
      message. Logging remains the same as in `Private`.
* **Frontend-Friendly Error Messages:** Supports converting error messages to your desired case convention, facilitating
  easier integration with frontend localization systems.
* **Standardized Error Responses for REST and SignalR:** Provides a standardized error response format, making it easier
  for frontend applications to parse and display error messages. The error response format for REST APIs is shown below:

```json
{
    "RequestId": "0HMVFE0A284AM:00000001",
    "TraceId": "a55582ab204162e66e124b0378776ab7",
    "Instance": "POST - 164.54.144.23:443/users/register",
    "StatusCode": 400,
    "Type": "BadRequestException",
    "Errors": {
        "email": "email_address_is_not_in_a_valid_format",
        "password": "password_must_be_at_least_8_characters_long"
    },
    "Message": "the_request_was_invalid_or_cannot_be_otherwise_served."
}
```

For SignalR, the standard error response format is:

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

## Installation

Install the package via NuGet Package Manager or use the following command:

```bash
Install-Package ResponseCrafter
```

## Usage

### 1. Setup Exception Handlers:

**Add** `AddResponseCrafter` in `program.cs` bby providing an optional naming convention, and
configure `ResponseCrafterVisibility` in your settings.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Basic setup
builder.AddResponseCrafter();

// Setup with a specific naming convention
builder.AddResponseCrafter(NamingConvention.ToUpperSnakeCase);

var app = builder.Build();
app.UseResponseCrafter();
app.Run();
```

Configure visibility in your `appsettings.json`:

```json
{
    "ResponseCrafterVisibility": "Public"
}
```

Supported naming conventions:

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

### 2. Setup Exception Handling for SignalR:

For SignalR support, register the exception filter for your hubs (or per hub) as follows:

```csharp
builder.Services.AddSignalR(options => options.AddFilter<SignalRExceptionFilter>());
```

If you already have the existing configuration for REST APIs, like
`builder.AddResponseCrafter(NamingConvention.ToSnakeCase);`, SignalR messages will automatically use the same error
handling and response crafting.

### 3. Implement Hub Methods with Standard Arguments:

To allow proper response handling, the hub methods should use the `HubArgument<T>` structure, which provides a unique
invocation ID for tracing errors and crafting detailed responses.

```csharp
public async Task SendMessage(HubArgument<Message> hubArgument)
{
    throw new BadRequestException("This is a test exception");
    await Clients.All.ReceiveMessage(hubArgument.Argument);
}
```

### 4. Define Custom Exceptions:

Create custom exception classes that inherit from `ApiException` or use the predefined ones. Use `ErrorDetails` records
for
specific error messages related to API requests.

### 5. Configure Middleware:

* Implement the exception handling middleware in your application's pipeline.

```csharp
app.UseResponseCrafter();
```

## SignalR-Specific Error Handling and Structure

When using the package with SignalR, the following structures are used for standardizing request and response handling:
**HubErrorResponse**
This class is used to format error responses sent back to the client:

```csharp
public class HubErrorResponse
{
   public required string InvocationId { get; set; }
   public required string Instance { get; set; }
   public int StatusCode { get; set; }
   public string Message { get; set; } = string.Empty;
   public Dictionary<string, string>? Errors { get; set; }
}
```

**HubArgument<T>**
This class wraps around the standard arguments passed to SignalR methods, adding an `InvocationId` to enable unique
error tracing.

```csharp
public class HubArgument<T>
{
   public required string InvocationId { get; set; }
   public required T Argument { get; set; }
}
```

## Logging and Error Responses:

The package automatically logs warnings or errors and provides crafted responses based on the exception type, whether
for REST APIs or SignalR hubs.

## Predefined HTTP Exceptions

* `BadRequestException`
* `UnauthorizedException`
* `PaymentRequiredException`
* `ForbiddenException`
* `NotFoundException`
* `ConflictException`
* `TooManyRequestsException`
* `InternalServerErrorException`
* `ServiceUnavailableException`

### Custom Exception Helper Methods

Using exception helpers:

```csharp
decimal? price = -10.5m;
//For 400 Bad Request
BadRequestException.ThrowIfNullOrNegative(price, "Price is negative");
//For 500 Internal Server Error
InternalServerErrorException.ThrowIfNullOrNegative(price, "Price is negative");

string? username = "   ";
//For 400 Bad Request
BadRequestException.ThrowIfNullOrWhiteSpace(username, "Please provide username");
//For 404 Not Found
NotFoundException.ThrowIfNullOrWhiteSpace(username);
//For 500 Internal Server Error
InternalServerErrorException.ThrowIfNullOrWhiteSpace(username, "Price is negative");

List<int> tags = [];
//For 400 Bad Request
BadRequestException.ThrowIfNullOrEmpty(tags, "Please provide tags");
//For 404 Not Found
NotFoundException.ThrowIfNullOrEmpty(tags);
//For 500 Internal Server Error
InternalServerErrorException.ThrowIfNullOrEmpty(tags, "Please provide tags");

object? user = null;
//For 400 Bad Request
BadRequestException.ThrowIfNull(user, "Please provide user");
//For 404 Not Found
NotFoundException.ThrowIfNull(user, "Please provide user");
//For 500 Internal Server Error
InternalServerErrorException.ThrowIfNull(user, "Please provide user");

bool userUnauthorized = false;
//For 401 Unauthorized
UnauthorizedException.ThrowIf(userUnauthorized, "User is unauthorized");
//For 500 Internal Server Error
InternalServerErrorException.ThrowIf(userUnauthorized, "User is unauthorized");
```

These examples show how to use the `ThrowIfNullOrNegative`, `ThrowIfNullOrWhiteSpace`, `ThrowIfNullOrEmpty` and
`ThrowIfNull` helper methods
from `BadRequestException`, `InternalServerErrorException` and `NotFoundException`. Adjust the object names and values
according to your specific
application needs.

## Recommendations

* **Error Message Formatting:** It's recommended to use snake_case for error messages to aid frontend applications in
  implementing localization.

## Limitations

* This package is specifically tailored for .NET 8 and above.

## License

Pandatech.ResponseCrafter is licensed under the MIT License.