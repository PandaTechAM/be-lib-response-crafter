namespace ResponseCrafter.Helpers;

/// <summary>
/// Generic class representing the hub method argument structure.
/// </summary>
/// <typeparam name="T">Type of the main argument value.</typeparam>
public class HubArgument<T>
{
   public required string InvocationId { get; set; }
   public required T Argument { get; set; }
}