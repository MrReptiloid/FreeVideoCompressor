using FreeVideoCompressor.Application.Services;
using FreeVideoCompressor.Domain.Abstractions;
using FreeVideoCompressor.Domain.Contracts;
using FreeVideoCompressor.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FreeVideoCompressor.Controllers;

[ApiController]
[Route("[controller]")]
public class CompressorController : ControllerBase
{
    private readonly IVideoProcessingService _videoProcessingService;
    private readonly IVideoValidationService _videoValidationService;
    private readonly CompressService _compressService;

    public CompressorController(
        IVideoProcessingService videoProcessingService,
        IVideoValidationService videoValidationService,
        CompressService compressService)
    {
        _videoProcessingService = videoProcessingService;
        _videoValidationService = videoValidationService;
        _compressService = compressService;
    }
    
    [HttpPost]
    [RequestSizeLimit(Constants.MaxVideoSize)] //500 MB limit of file size
    public async Task<IActionResult> UploadVideo([FromForm]UploadVideoRequest request)
    {
        var validationResult = _videoValidationService.ValidateVideo(request.VideoFile);
        if (validationResult.IsErr)
        {
            return BadRequest(validationResult.UnwrapErr());
        }

        var processResult = await _videoProcessingService.ProcessVideoAsync(request.VideoFile);
        if (processResult.IsErr)
        {
            return BadRequest(processResult.UnwrapErr());
        }

        var (fileId, fullPath) = processResult.Unwrap();
        var flowResult = await _compressService.InitFlow(fileId, fullPath);

        return flowResult.Match<IActionResult>(
            ok: _ => Ok(new UploadVideoResponse { FileId = fileId }),
            err: errorMessage => BadRequest(new UploadVideoResponse { Message = errorMessage })
        );
        
    }
}
