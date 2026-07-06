namespace ResponseCrafter.ExceptionHandlers.SignalR;

/// <summary>Contract for hub method arguments that carry an invocation id for error correlation.</summary>
public interface IHubArgument
{
    /// <summary>Client-supplied id correlating the invocation with its error response.</summary>
    public string InvocationId { get; set; }
}
