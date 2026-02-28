using System.Text;
using Microsoft.AspNetCore.Http;

namespace ResponseCrafter.Helpers;

/// <summary>
/// Helper methods for building exception messages and extracting request information.
/// </summary>
internal static class ExceptionMessageBuilder
{
   /// <summary>
   /// Creates a verbose exception message including all inner exceptions.
   /// </summary>
   public static string CreateVerboseExceptionMessage(this Exception exception)
   {
      var stringBuilder = new StringBuilder();

      stringBuilder.AppendLine("Exception Message:");
      stringBuilder.AppendLine(exception.Message);

      stringBuilder.AppendLine("Stack Trace:");
      stringBuilder.AppendLine(exception.StackTrace);

      var innerException = exception.InnerException;
      while (innerException != null)
      {
         stringBuilder.AppendLine("Inner Exception:");
         stringBuilder.AppendLine(innerException.Message);

         stringBuilder.AppendLine("Inner Exception Stack Trace:");
         stringBuilder.AppendLine(innerException.StackTrace);

         innerException = innerException.InnerException;
      }

      return stringBuilder.ToString();
   }

   /// <summary>
   /// Creates a request path string in the format "METHOD /path".
   /// </summary>
   internal static string CreateRequestPath(HttpContext context)
   {
      return $"{context.Request.Method} {context.Request.Path}{context.Request.QueryString}";
   }
}