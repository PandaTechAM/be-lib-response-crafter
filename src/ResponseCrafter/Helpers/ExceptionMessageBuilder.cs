using System.Text;
using Microsoft.AspNetCore.Http;

namespace ResponseCrafter.Helpers;

public static class ExceptionMessageBuilder
{
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

   public static string CreateRequestPath(HttpContext context)
   {
      return $"{context.Request.Method} - {context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
   }
}