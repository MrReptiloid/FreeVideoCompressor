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
        
        return await _compressVideoFlowRepository.CreateAsync(compressVideoFlow)
            .MapErr(err => $"Failed to create compress flow: {err}")
            .Map(_ => compressVideoFlow);
    }

    public async Task<Result<Unit, string>> StartCompressAsync(Guid flowId, CancellationToken cancellationToken)
    {
        return await _compressVideoFlowRepository.ReadAsync(flowId)
            .Bind(flow => EnsureFlowStatus(flow, CompressVideoFlowStatus.Uploaded))
            .BindAsync(async flow => await EnqueueCompressionAsync(flow, cancellationToken));
    }
    public async Task<Result<string, string>> GetLocalOutputPathAsync(Guid flowId, CancellationToken cancellationToken)
    {
        return await _compressVideoFlowRepository.ReadAsync(flowId)
            .Bind(flow => EnsureFlowExists(flow))
            .Bind(flow => EnsureFlowStatus(flow, CompressVideoFlowStatus.Completed))
            .Map(_ => Path.Combine(Constants.UploadsPath, flowId.ToString()));
    }
    
    public  async Task OnCompressionCompleted(Guid flowId)
    {
        await _compressVideoFlowRepository.PatchStatusAsync(flowId, CompressVideoFlowStatus.Completed);
    }
    
    private Result<CompressVideoFlow, string> EnsureFlowStatus(CompressVideoFlow flow, CompressVideoFlowStatus status)
    {
        return flow.Status == status
            ? Result<CompressVideoFlow, string>.Ok(flow)
            : Result<CompressVideoFlow, string>.Err("Wrong status");
    }
    
    private Result<CompressVideoFlow, string> EnsureFlowExists(CompressVideoFlow? flow)
    {
        return flow != null
            ? Result<CompressVideoFlow, string>.Ok(flow)
            : Result<CompressVideoFlow, string>.Err("No compress flow found");
    }
    
    private async Task<Result<Unit, string>> EnqueueCompressionAsync(CompressVideoFlow flow, CancellationToken cancellationToken)
    {
        await _compressVideoFlowRepository.PatchStatusAsync(flow.Id, CompressVideoFlowStatus.Processing);

        var outputFilePath = $"{flow.InputFilePath}_compressed.mp4";

        var jobId = _jobClient.Enqueue<FfmpegService>(fs =>
            fs.CompressAsync(flow.InputFilePath, outputFilePath, cancellationToken));

        _jobClient.ContinueJobWith<CompressService>(jobId, cs =>
            cs.OnCompressionCompleted(flow.Id));

        return Result<Unit, string>.Ok(Unit.Value);
    }
}