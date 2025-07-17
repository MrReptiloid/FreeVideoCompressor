using FreeVideoCompressor.Application.Services;
using FreeVideoCompressor.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FreeVideoCompressor.Controllers;

[ApiController]
[Route("[controller]")]
[RequestSizeLimit(500 * 1024 * 1024)] //500 MB limit of file size
public class CompressorController : ControllerBase
{
    private readonly string[] _supportedVideoFormats = [ ".mp4", ".avi", ".mov", ".mkv", ".flv", ".wmv", ".webm" ];
    private readonly FileService _fileService;

    public CompressorController(FileService fileService)
    {
        _fileService = fileService;
    }
    
    [HttpPost]
    public async Task<IActionResult> UploadVideo([FromForm]UploadVideoRequest request)
    {
        if (request.VideoFile.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        if (!_supportedVideoFormats.Contains(Path.GetExtension(request.VideoFile.FileName)))
        {
            return BadRequest("Unsupported file format. Supported formats are: " + string.Join(", ", _supportedVideoFormats));
        }
        
        string result = await _fileService.SaveFileAsync(request.VideoFile);
        
        return Ok(new { Message = result });
    }
}
