using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ResponseCrafter.HttpExceptions;

public class InternalServerErrorException : ApiException
{
   private const string DefaultMessage = "an_internal_server_error_occurred.";

   public InternalServerErrorException(string message, Dictionary<string, string>? errors = null)
      : base(500, message, errors)
   {
   }

   public InternalServerErrorException(Dictionary<string, string> errors)
      : base(500, DefaultMessage, errors)
   {
   }

   public static void ThrowIfNull([NotNull] object? value, string exceptionMessage)
   {
      if (value is null)
      {
         throw new InternalServerErrorException(exceptionMessage);
      }
   }

   public static void ThrowIfNull([NotNull] object? value, string exceptionMessage, Dictionary<string, string>? errors = null)
   {
      if (value is null)
      {
         throw new InternalServerErrorException(exceptionMessage, errors);
      }
   }

   public static void ThrowIfNullOrEmpty([NotNull] IEnumerable? value, string exceptionMessage)
   {
      // ReSharper disable once GenericEnumeratorNotDisposed
      if (value is null || !value.GetEnumerator()
                                 .MoveNext())
      {
         throw new InternalServerErrorException(exceptionMessage);
      }
   }

   public static void ThrowIfNullOrEmpty([NotNull] IEnumerable? value, string exceptionMessage, Dictionary<string, string>? errors = null)
   {
      if (value is null || !value.GetEnumerator()
                                 .MoveNext())
      {
         throw new InternalServerErrorException(exceptionMessage, errors);
      }
   }

   public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage)
   {
      if (string.IsNullOrWhiteSpace(value))
      {
         throw new InternalServerErrorException(exceptionMessage);
      }
   }

   public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, string exceptionMessage, Dictionary<string, string>? errors = null)
   {
      if (string.IsNullOrWhiteSpace(value))
      {
         throw new InternalServerErrorException(exceptionMessage, errors);
      }
   }

   public static void ThrowIf(bool condition, string exceptionMessage)
   {
      if (condition)
      {
         throw new InternalServerErrorException(exceptionMessage);
      }
   }

   public static void ThrowIf(bool condition, string exceptionMessage, Dictionary<string, string>? errors = null)
   {
      if (condition)
      {
         throw new InternalServerErrorException(exceptionMessage, errors);
      }
   }
   public static void ThrowIfNullOrNegative([NotNull] decimal? value, string exceptionMessage)
   {
      if (value is < 0 or null)
      {
         throw new InternalServerErrorException(exceptionMessage);
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