namespace FreeVideoCompressor.Domain.Utilities;

public abstract record FailureBase(string Message, Exception? InnerException = null)
{
    protected DateTime Timestamp { get; } = DateTime.UtcNow;
}
