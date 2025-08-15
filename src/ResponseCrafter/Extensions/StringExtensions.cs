using Humanizer;
using ResponseCrafter.Enums;
using ResponseCrafter.Options;

namespace ResponseCrafter.Extensions;

public static class StringExtensions
{
   public static string ConvertCase(this string message, NamingConvention namingConvention)
   {
      return namingConvention switch
      {
         NamingConvention.Default => message,
         NamingConvention.ToSnakeCase => message.Underscore(),
         NamingConvention.ToPascalCase => message.Underscore()
                                                 .Pascalize(),
         NamingConvention.ToCamelCase => message.Underscore()
                                                .Camelize(),
         NamingConvention.ToKebabCase => message.Underscore()
                                                .Kebaberize(),
         NamingConvention.ToTitleCase => message.Underscore()
                                                .Titleize(),
         NamingConvention.ToHumanCase => message.Underscore()
                                                .Humanize(),
         NamingConvention.ToUpperSnakeCase => message.Underscore()
                                                     .ToUpper(),
         _ => message
      };
   }

   internal static string ConvertCase(this string message, NamingConventionOptions option)
   {
      return message.ConvertCase(option.NamingConvention);
   }

   public static Dictionary<string, string>? ConvertCase(this Dictionary<string, string>? errors,
      NamingConvention namingConvention)
   {
      return errors?.ToDictionary(
         kvp => kvp.Key.ConvertCase(namingConvention),
         kvp => kvp.Value.ConvertCase(namingConvention));
   }

   internal static Dictionary<string, string>? ConvertCase(this Dictionary<string, string>? errors,
      NamingConventionOptions option) =>
      errors.ConvertCase(option.NamingConvention);
}