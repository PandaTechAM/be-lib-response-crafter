using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ResponseCrafter.HttpExceptions;

public class GatewayTimeoutException : ApiException
{
   private const string DefaultMessage = "the_request_timed_out_while_waiting_for_an_upstream_server.";

   public GatewayTimeoutException(string message = DefaultMessage, Dictionary<string, string>? errors = null)
      : base(504, message, errors)
   {
   }

   public static void ThrowIfNull([NotNull] object? value, string exceptionMessage, Dictionary<string, string>? errors = null)
   {
      if (value is null)
      {
         throw new GatewayTimeoutException(exceptionMessage, errors);
      }
   }

   public static void ThrowIfNullOrEmpty([NotNull] IEnumerable? value, string exceptionMessage, Dictionary<string, string>? errors = null)
   {
      if (value is null || !value.GetEnumerator().MoveNext())
      {
         throw new GatewayTimeoutException(exceptionMessage, errors);
      }
   }

   public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage, Dictionary<string, string>? errors = null)
   {
      if (string.IsNullOrWhiteSpace(value))
      {
         throw new GatewayTimeoutException(exceptionMessage, errors);
      }
   }

   public static void ThrowIf(bool condition, string exceptionMessage, Dictionary<string, string>? errors = null)
   {
      if (condition)
      {
         throw new GatewayTimeoutException(exceptionMessage, errors);
      }
   }

   public static void ThrowIfNullOrNegative([NotNull] decimal? value, string exceptionMessage, Dictionary<string, string>? errors = null)
   {
      if (value is < 0 or null)
      {
         throw new GatewayTimeoutException(exceptionMessage, errors);
      }
   }
}
