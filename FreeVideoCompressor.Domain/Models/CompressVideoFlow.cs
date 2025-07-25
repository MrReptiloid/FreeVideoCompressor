namespace FreeVideoCompressor.Domain.Models;

public enum CompressVideoFlowStatus
{
    Uploaded,
    Compressing,
    Completed,
}

public record CompressVideoFlow
{
    public Guid Id { get; init; }
    public CompressVideoFlowStatus Status { get; init; }
    public string FileId { get; init; } = string.Empty!;
    public string InputFilePath { get; init; } = string.Empty!;
}