using Humanizer;
using ResponseCrafter.Enums;
using ResponseCrafter.Options;

namespace ResponseCrafter.Extensions;

/// <summary>
///     Extension methods for string and dictionary case conversion based on naming conventions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     Converts a string to the specified naming convention.
    /// </summary>
    public static string ConvertCase(this string message, NamingConvention namingConvention)
    {
        return namingConvention switch
        {
            NamingConvention.Default => message,
            NamingConvention.ToSnakeCase => message.Underscore(),
            NamingConvention.ToPascalCase => message.Underscore().Pascalize(),
            NamingConvention.ToCamelCase => message.Underscore().Camelize(),
            NamingConvention.ToKebabCase => message.Underscore().Kebaberize(),
            NamingConvention.ToTitleCase => message.Underscore().Titleize(),
            NamingConvention.ToHumanCase => message.Underscore().Humanize(),
            NamingConvention.ToUpperSnakeCase => message.Underscore().ToUpper(),
            _ => message
        };
    }

    /// <summary>
    ///     Converts a string to the specified naming convention from options.
    /// </summary>
    internal static string ConvertCase(this string message, NamingConventionOptions option)
    {
        return message.ConvertCase(option.NamingConvention);
    }

    /// <summary>
    ///     Converts dictionary keys and values to the specified naming convention.
    /// </summary>
    public static Dictionary<string, string>? ConvertCase(this Dictionary<string, string>? errors,
        NamingConvention namingConvention)
    {
        return errors?.ToDictionary(
            kvp => kvp.Key.ConvertCase(namingConvention),
            kvp => kvp.Value.ConvertCase(namingConvention));
    }

    /// <summary>
    ///     Converts dictionary keys and values to the specified naming convention from options.
    /// </summary>
    internal static Dictionary<string, string>? ConvertCase(this Dictionary<string, string>? errors,
        NamingConventionOptions option)
    {
        return errors.ConvertCase(option.NamingConvention);
    }
}
