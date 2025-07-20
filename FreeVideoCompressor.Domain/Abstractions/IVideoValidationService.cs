using FreeVideoCompressor.Domain.Failures;
using FreeVideoCompressor.Domain.Utilities;
using Microsoft.AspNetCore.Http;

namespace FreeVideoCompressor.Domain.Abstractions;

public interface IVideoValidationService
{
    Result<Unit, FileFailure> ValidateVideo(IFormFile videoFile);
}