# Pandatech.ResponseCrafter

## Introduction

ResponseCrafter is a comprehensive NuGet package for .NET 8+, specifically designed for enhanced exception handling and
logging in ASP.NET applications. This package simplifies the process of handling standard and custom exceptions by
crafting detailed error responses suitable for both development and production environments.

## Features

* **Custom Exception Handling:** Streamlines the process of managing both standard HTTP exceptions and custom
  exceptions.
* **Detailed Error Responses:** Generates verbose error messages, including stack traces for in-depth debugging in
  development environments.
* **Environment-Sensitive Logging:** Offers a class `PandaExceptionHandler` which can be configured for message verbosity.
    In production environments, only the exception type and message are logged. In development environments, the entire
    exception is logged, including the stack trace.
* **Frontend-Friendly Error Messages:** Encourages the use of snake_case in error messages, facilitating easier
  integration with frontend localization systems.
* **Organized/Readable and standardized error responses:** Provides a standardized error response format for all
  exceptions, making it easier for frontend applications to parse and display error messages. The error response format is shown below:
```json
{
  "TraceId": "0HMVFE0A284AM:00000001",
  "Instance": "POST - 164.54.144.23:443/users/register",
  "StatusCode": 400,
  "Type": "BadRequestException",
  "Errors": {
    "email": "email_address_is_not_in_a_valid_format",
    "password": "password_must_be_at_least_8_characters_long"
  },
  "Message": "the_request_was_invalid_or_cannot_be_otherwise_served."
}

````

## Installation

Install the package via NuGet Package Manager or use the following command:

```bash
Install-Package ResponseCrafter
```

## Usage

### 1. Setup Exception Handlers:

**Add** `PandaExceptionHandler` in program.cs and in configuration set `"ResponseCrafterVisibility"` to `"Public"` or `"Private"`.

```csharp
builder.Services.AddExceptionHandler<PandaExceptionHandler>();
```
```json
{
  "ResponseCrafterVisibility": "Public"
}

```

### 2. Define Custom Exceptions:

* Create a custom exception class that inherits from `ApiException` or use already created ones:

* Utilize `ErrorDetails` records for specific error messages related to API requests.

### 3. Configure Middleware:

* Implement the exception handling middleware in your application's pipeline.

```csharp
app.UseExceptionHandlerMiddleware(_ => { }); //the lambda parameter is not needed it is just .net 8 bug which might be fixed in the future
```

### 4. Logging and Error Responses:

* Automatically logs warnings or errors and provides crafted responses base on the exception type.

## Custom Exception Already Created

* `BadRequestException`
* `UnauthorizedException`
* `PaymentRequiredException`
* `ForbiddenException`
* `NotFoundException`
* `ConflictException`
* `TooManyRequestsException`
* `InternalServerErrorException`
* `ServiceUnavailableException`

## Recommendations

* **Error Message Formatting:** It's recommended to use snake_case for error messages to aid frontend applications in
  implementing localization.

## Limitations

* This package is specifically tailored for .NET 8 and above.

## License

Pandatech.ResponseCrafter is licensed under the MIT License.