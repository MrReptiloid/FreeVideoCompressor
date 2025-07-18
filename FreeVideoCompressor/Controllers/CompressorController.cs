using FreeVideoCompressor.Application.Services;
using FreeVideoCompressor.Domain.Abstractions;
using FreeVideoCompressor.Domain.Contracts;
using FreeVideoCompressor.Domain.Failures;
using FreeVideoCompressor.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FreeVideoCompressor.Controllers;

[ApiController]
[Route("[controller]")]
public class CompressorController : ControllerBase
{
    private readonly string[] _supportedVideoFormats = [ ".mp4", ".avi", ".mov", ".mkv", ".flv", ".wmv", ".webm" ];
    private readonly IFileService _fileService;
    private readonly CompressService _compressService;

    public CompressorController(IFileService fileService, CompressService compressService)
    {
        _fileService = fileService;
        _compressService = compressService;
    }
    
    [HttpPost]
    [RequestSizeLimit(500 * 1024 * 1024)] //500 MB limit of file size
    public async Task<IActionResult> UploadVideo([FromForm]UploadVideoRequest request)
    {
        if (request.VideoFile.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }
        
        string fileExtension = Path.GetExtension(request.VideoFile.FileName).ToLowerInvariant();
        if (!IsValidVideoFormat(fileExtension))
        {
            return BadRequest($"Unsupported video format: {fileExtension}. Supported formats are: {string.Join(", ", _supportedVideoFormats)}");
        }
        
        Result<string, string> processResult = await ProcessVideoAsync(request.VideoFile);

        if (processResult.IsErr)
        {
            return BadRequest(processResult.UnwrapErr());
        }

        Result<Unit, string> flowResult = await _compressService.InitFlow(processResult.Unwrap());
        
        return flowResult.Match<IActionResult>(
            ok: _ => Ok("Video uploaded successfully."),
            err: errorMessage => BadRequest(errorMessage)
        );
    }

    private async Task<Result<string, string>> ProcessVideoAsync(IFormFile file)
    {
        try
        {
            using MemoryStream stream = new MemoryStream();
            await file.CopyToAsync(stream);
            byte[] bytes = stream.ToArray();

            string wwwrootDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

            Result<string, FileFailure> saveResult = await _fileService.SaveFileAsync(bytes, wwwrootDir, file.FileName);

            return saveResult.Match(
                ok: success => Result<string, string>.Ok(success),
                err: failure => Result<string, string>.Err($"{failure.BaseMessage}\n{failure.Message}")
            );
        }
        catch (Exception ex)
        {
            return Result<string, string>.Err($"An error occurred while processing the video: {ex.Message}");
        }
    }
    
    private bool IsValidVideoFormat(string extension)
    {
        return Array.Exists(_supportedVideoFormats, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase));
    }
}
