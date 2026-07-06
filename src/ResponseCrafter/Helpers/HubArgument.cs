namespace ResponseCrafter.Helpers;

/// <summary>
///     Generic class representing the hub method argument structure.
/// </summary>
/// <typeparam name="T">Type of the main argument value.</typeparam>
public class HubArgument<T>
{
    /// <summary>Client-supplied id correlating the invocation with its error response.</summary>
    public required string InvocationId { get; set; }

    /// <summary>Main argument value passed to the hub method.</summary>
    public required T Argument { get; set; }
}
