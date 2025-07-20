namespace FreeVideoCompressor.Domain.Contracts;

public record UploadVideoResponse
{
    public string? Message { get; init; } 
    public string? FileId { get; init; }
}