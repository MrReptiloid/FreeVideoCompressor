using FreeVideoCompressor.DataAccess.Repositories;
using FreeVideoCompressor.Domain.Models;
using FreeVideoCompressor.Domain.Utilities;
using Hangfire;

namespace FreeVideoCompressor.Application.Services;

public class CompressService
{
    private readonly CompressVideoFlowRepository _compressVideoFlowRepository;
    private readonly IBackgroundJobClient _jobClient;
    
    public CompressService(
        CompressVideoFlowRepository compressVideoFlowRepository,
        IBackgroundJobClient jobClient)
    {
        _compressVideoFlowRepository = compressVideoFlowRepository;
        _jobClient = jobClient;
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

    public async Task<Result<Unit, string>> StartCompressAsync(Guid flowId, CancellationToken cancellationToken)
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

        Result<Unit, string> patchResult =
            await _compressVideoFlowRepository.PatchStatusAsync(compressVideoFlow.Id,
                CompressVideoFlowStatus.Processing);
        if (patchResult.IsErr)
        {
            return Result<Unit, string>.Err($"Failed to update compress flow status: {patchResult.UnwrapErr()}");
        }
        
        string jobId = _jobClient.Enqueue<FfmpegService>(fs =>
            fs.CompressAsync(compressVideoFlow.InputFilePath, outputFilePath, cancellationToken));
        
        _jobClient.ContinueJobWith<CompressService>(jobId, ns => OnCompressionCompleted(compressVideoFlow.Id));
        
        return Result<Unit, string>.Ok(Unit.Value);
    }
    
    public async Task OnCompressionCompleted(Guid flowId)
    {
        await _compressVideoFlowRepository.PatchStatusAsync(flowId, CompressVideoFlowStatus.Completed);
    }
}