namespace FreeVideoCompressor.Domain.Utilities;

public abstract record FailureBase(string BaseMessage, string? Message, Exception? InnerException = null)
{
    protected DateTime Timestamp { get; } = DateTime.UtcNow;

    public override string ToString()
    {
        return $"{BaseMessage} - {Message ?? "No additional information"}";
    }
}
