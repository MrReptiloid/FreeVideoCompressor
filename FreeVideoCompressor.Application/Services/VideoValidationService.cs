using FreeVideoCompressor.Domain.Abstractions;
using FreeVideoCompressor.Domain.Failures;
using FreeVideoCompressor.Domain.Utilities;
using Microsoft.AspNetCore.Http;

namespace FreeVideoCompressor.Application.Services;

public class VideoValidationService : IVideoValidationService
{
    private readonly HashSet<string> _supportedVideoFormats = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp4", ".avi", ".mov", ".mkv", ".flv", ".wmv", ".webm"
    };

    public Result<Unit, FileFailure> ValidateVideo(IFormFile videoFile)
    {
        if (videoFile.Length == 0)
        {
            return Result<Unit, FileFailure>.Err(FileFailure.NotFound());
        }

        string fileExtension = Path.GetExtension(videoFile.FileName).ToLowerInvariant();
        if (!IsValidVideoFormat(fileExtension))
        {
            return Result<Unit, FileFailure>
                .Err(FileFailure.UnsupportedFormat(
                    $"Unsupported video format: {fileExtension}. Supported formats are: {string.Join(", ", _supportedVideoFormats)}"));
        }

        if (videoFile.Length > Constants.MaxVideoSize)
        {
            return Result<Unit, FileFailure>
                .Err(FileFailure.TooLarge(
                    $"Video file size exceeds the maximum limit of {Constants.MaxVideoSize / (1024 * 1024)} MB."));
        }

        return Result<Unit, FileFailure>.Ok(Unit.Value);
    }

    private bool IsValidVideoFormat(string extension)
    {
        return _supportedVideoFormats.Contains(extension);
    }
}