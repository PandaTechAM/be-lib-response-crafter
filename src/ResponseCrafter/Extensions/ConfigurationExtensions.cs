using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ResponseCrafter.Extensions;

internal static class ConfigurationExtensions
{
   private const string ResponseCrafterVisibility = "ResponseCrafterVisibility";

   public static string GetResponseCrafterVisibility<T>(this IConfiguration configuration, ILogger<T> logger)
   {
      var visibility = configuration[ResponseCrafterVisibility];

      if (!string.IsNullOrWhiteSpace(visibility) && visibility is "Private" or "Public")
      {
         return visibility;
      }

      logger.LogWarning("Visibility configuration was not available. Defaulted to 'Public'.");
      return "Public";
   }
}