using Humanizer;
using ResponseCrafter.Enums;
using ResponseCrafter.Options;

namespace ResponseCrafter.Extensions;

public static class StringExtensions
{
   extension(string message)
   {
      public string ConvertCase(NamingConvention namingConvention)
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

      internal string ConvertCase(NamingConventionOptions option)
      {
         return message.ConvertCase(option.NamingConvention);
      }
   }

   extension(Dictionary<string, string>? errors)
   {
      public Dictionary<string, string>? ConvertCase(NamingConvention namingConvention)
      {
         return errors?.ToDictionary(
            kvp => kvp.Key.ConvertCase(namingConvention),
            kvp => kvp.Value.ConvertCase(namingConvention));
      }

      internal Dictionary<string, string>? ConvertCase(NamingConventionOptions option) =>
         errors.ConvertCase(option.NamingConvention);
   }
}