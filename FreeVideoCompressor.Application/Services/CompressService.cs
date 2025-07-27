using FreeVideoCompressor.DataAccess.Repositories;
using FreeVideoCompressor.Domain.Models;
using FreeVideoCompressor.Domain.Utilities;

namespace FreeVideoCompressor.Application.Services;

public class CompressService
{
    private readonly CompressVideoFlowRepository _compressVideoFlowRepository;
    private readonly FfmpegService _ffmpegService;
    
    public CompressService(
        CompressVideoFlowRepository compressVideoFlowRepository,
        FfmpegService ffmpegService)
    {
        _compressVideoFlowRepository = compressVideoFlowRepository;
        _ffmpegService = ffmpegService;
    }
    
    public async Task<Result<CompressVideoFlow, string>> InitFlow(string fileId, string filePath)
    {
        CompressVideoFlow compressVideoFlow = new(){
            Id = Guid.NewGuid(),
            Status = CompressVideoFlowStatus.Uploaded,
            FileId = fileId,
            InputFilePath = filePath
        };
        
        Result<Unit, string> createResult = await _compressVideoFlowRepository.CreateAsync(compressVideoFlow);
        if (createResult.IsErr)
        {
            return Result<CompressVideoFlow, string>.Err($"Failed to create compress flow: {createResult.UnwrapErr()}");
        }
        
        return Result<CompressVideoFlow, string>.Ok(compressVideoFlow);
    }

    public async Task<Result<Unit, string>> StartCompressAsync(Guid flowId)
    {
        Result<CompressVideoFlow?, string> getFlowResult = await _compressVideoFlowRepository.ReadAsync(flowId);
        if (getFlowResult.IsErr || getFlowResult.Unwrap() == null)
        {
            return Result<Unit, string>.Err("No compress flow found");
        }
        
        CompressVideoFlow compressVideoFlow = getFlowResult.Unwrap()!;
        if (compressVideoFlow.Status != CompressVideoFlowStatus.Uploaded)
        {
            return Result<Unit, string>.Err("Compress flow is not in the correct state to start compression");
        }
        
        string outputFilePath = Path.Combine($"{compressVideoFlow.InputFilePath}_compressed.mp4");
        await _ffmpegService.CompressAsync(compressVideoFlow.InputFilePath, outputFilePath);
        
        return Result<Unit, string>.Ok(Unit.Value);
    }
    
}