namespace FreeVideoCompressor.Domain.Contracts;

public sealed record StartProcessingRequest
(
    Guid FlowId
);