namespace ResponseCrafter.Enums;

/// <summary>Naming convention applied to error messages and field keys before they are returned to clients.</summary>
public enum NamingConvention
{
    /// <summary>Leave messages and keys unchanged.</summary>
    Default = 0,

    /// <summary>Convert to snake_case.</summary>
    ToSnakeCase = 1,

    /// <summary>Convert to PascalCase.</summary>
    ToPascalCase = 2,

    /// <summary>Convert to camelCase.</summary>
    ToCamelCase = 3,

    /// <summary>Convert to kebab-case.</summary>
    ToKebabCase = 4,

    /// <summary>Convert to Title Case.</summary>
    ToTitleCase = 5,

    /// <summary>Convert to human-readable sentence case.</summary>
    ToHumanCase = 6,

    /// <summary>Convert to UPPER_SNAKE_CASE.</summary>
    ToUpperSnakeCase = 7
}
