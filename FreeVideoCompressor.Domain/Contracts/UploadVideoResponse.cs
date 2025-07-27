namespace FreeVideoCompressor.Domain.Contracts;

public record UploadVideoResponse
{
    public string? Message { get; init; } 
    public Guid? FlowId { get; init; }
}