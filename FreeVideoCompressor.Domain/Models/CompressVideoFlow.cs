namespace FreeVideoCompressor.Domain.Models;

public enum CompressVideoFlowStatus
{
    Uploaded,
    Processing,
    Completed,
}

public record CompressVideoFlow
{
    public Guid Id { get; init; }
    public CompressVideoFlowStatus Status { get; set; }
    public string FileId { get; init; } = string.Empty!;
    public string InputFilePath { get; init; } = string.Empty!;
}