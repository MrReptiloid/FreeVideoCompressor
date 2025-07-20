using FreeVideoCompressor.Domain.Utilities;
using Microsoft.AspNetCore.Http;

namespace FreeVideoCompressor.Domain.Abstractions;

public interface IVideoProcessingService
{
    Task<Result<(string FileId, string FullPath), string>> ProcessVideoAsync(IFormFile file);
}