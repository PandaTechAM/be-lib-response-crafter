using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ResponseCrafter.HttpExceptions;

public class BadRequestException : ApiException
{
   private const string DefaultMessage = "the_request_was_invalid_or_cannot_be_otherwise_served.";

   public BadRequestException(string message, Dictionary<string, string>? errors = null)
      : base(400, message, errors)
   {
   }

   public BadRequestException(Dictionary<string, string> errors)
      : base(400, DefaultMessage, errors)
   {
   }

   public static void ThrowIfNull([NotNull] object? value, string exceptionMessage)
   {
      if (value is null)
      {
         throw new BadRequestException(exceptionMessage);
      }
   }

   public static void ThrowIfNull([NotNull] object? value, string exceptionMessage, Dictionary<string, string>? errors = null) 
   {
      if (value is null)
      {
         throw new BadRequestException(exceptionMessage, errors);
      }
   }

   public static void ThrowIfNullOrEmpty([NotNull] IEnumerable? value, string exceptionMessage)
   {
      // ReSharper disable once GenericEnumeratorNotDisposed
      if (value is null || !value.GetEnumerator()
                                 .MoveNext())
      {
         throw new BadRequestException(exceptionMessage);
      }
   }

   public static void ThrowIfNullOrEmpty([NotNull] IEnumerable? value, string exceptionMessage, Dictionary<string, string>? errors = null)
   {
      if (value is null || !value.GetEnumerator()
                                 .MoveNext())
      {
         throw new BadRequestException(exceptionMessage, errors);
      }
   }

   public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage)
   {
      if (string.IsNullOrWhiteSpace(value))
      {
         throw new BadRequestException(exceptionMessage);
      }
   }

   public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage, Dictionary<string, string>? errors = null)
   {
      if (string.IsNullOrWhiteSpace(value))
      {
         throw new BadRequestException(exceptionMessage, errors);
      }
   }

   public static void ThrowIf(bool condition, string exceptionMessage)
   {
      if (condition)
      {
         throw new BadRequestException(exceptionMessage);
      }
   }

   public static void ThrowIf(bool condition, string exceptionMessage, Dictionary<string, string>? errors = null)
   {
      if (condition)
      {
         throw new BadRequestException(exceptionMessage, errors);
      }
   }

   public static void ThrowIfNullOrNegative([NotNull] decimal? value, string exceptionMessage)
   {
      if (value is < 0 or null)
      {
         throw new BadRequestException(exceptionMessage);
      }
   }

   public static void ThrowIfNullOrNegative([NotNull] decimal? value, string exceptionMessage, Dictionary<string, string>? errors = null)
   {
      if (value is < 0 or null)
      {
         throw new BadRequestException(exceptionMessage, errors);
      }
   }
}