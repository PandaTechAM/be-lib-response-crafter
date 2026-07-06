using ResponseCrafter.Enums;

namespace ResponseCrafter.Options;

/// <summary>Options controlling the naming convention applied to outgoing error responses.</summary>
public class NamingConventionOptions
{
    /// <summary>Naming convention applied to error messages and field keys.</summary>
    public NamingConvention NamingConvention { get; set; }
}
