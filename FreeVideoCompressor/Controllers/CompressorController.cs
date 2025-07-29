using FreeVideoCompressor.Application.Services;
using FreeVideoCompressor.Domain.Abstractions;
using FreeVideoCompressor.Domain.Contracts;
using FreeVideoCompressor.Domain.Failures;
using FreeVideoCompressor.Domain.Models;
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
        Result<Unit, FileFailure> validationResult = _videoValidationService.ValidateVideo(request.VideoFile);
        if (validationResult.IsErr)
        {
            return BadRequest(validationResult.UnwrapErr());
        }

        Result<(string FileId, string FullPath), string> processResult =
            await _videoProcessingService.ProcessVideoAsync(request.VideoFile);
        if (processResult.IsErr)
        {
            return BadRequest(processResult.UnwrapErr());
        }

        (string fileId, string fullPath) = processResult.Unwrap();
        Result<CompressVideoFlow, string> flowResult = await _compressService.InitFlow(fileId, fullPath);

        return flowResult.Match<IActionResult>(
            ok: flow => Ok(new UploadVideoResponse { FlowId = flow.Id }),
            err: errorMessage => BadRequest(new UploadVideoResponse { Message = errorMessage })
        );
    }
    
    [HttpGet("{flowId}")]
    public async Task<IActionResult> StartProcessing(string flowId, CancellationToken cancellationToken)
    {
        Guid flowGuid = Guid.Parse(flowId);
        Result<Unit, string> flowResult = await _compressService.StartCompressAsync(flowGuid, cancellationToken);
        return flowResult.Match<IActionResult>(
            ok: _ => Ok(),
            err: errorMessage => BadRequest(new { Message = errorMessage })
        );
    }
}
