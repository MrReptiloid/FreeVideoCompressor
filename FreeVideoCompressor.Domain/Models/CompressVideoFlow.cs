namespace FreeVideoCompressor.Domain.Models;

public enum CompressVideoFlowStatus
{
    Uploaded,
    Compressing,
    Completed,
}

public record CompressVideoFlow(
    Guid Id,
    CompressVideoFlowStatus Status,
    string InputFilePath);