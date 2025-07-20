using FreeVideoCompressor.Domain.Abstractions;
using FreeVideoCompressor.Domain.Failures;
using FreeVideoCompressor.Domain.Utilities;
using Microsoft.AspNetCore.Http;

namespace FreeVideoCompressor.Application.Services;

public class VideoProcessingService : IVideoProcessingService
{
    private readonly IFileService _fileService;

    public VideoProcessingService(IFileService fileService)
    {
        _fileService = fileService;
    }
    
    public async Task<Result<(string FileId, string FullPath), string>> ProcessVideoAsync(IFormFile file)
    {
        try
        {
            using MemoryStream stream = new();
            await file.CopyToAsync(stream);
            byte[] fileBytes = stream.ToArray();
            
            string workDirPath = Path.Combine(Directory.GetCurrentDirectory(), Constants.UploadsPath);
            string fileId = Guid.NewGuid().ToString();
            
            Result<string, FileFailure> saveResult = await _fileService.SaveFileAsync(
                fileBytes,
                workDirPath,
                $"{fileId}{Path.GetExtension(file.FileName)}");

            return saveResult.Match(
                ok: _ => Result<(string FileId, string FullPath), string>.Ok((fileId, saveResult.Unwrap())),
                err: failure => Result<(string fileId, string FullPath), string>
                    .Err(failure.ToString()));
        }
        catch (Exception ex)
        {
            return Result<(string FileId, string FullPath), string>
                .Err($"An error occurred while processing the video: {ex.Message}");
        }
    }
}