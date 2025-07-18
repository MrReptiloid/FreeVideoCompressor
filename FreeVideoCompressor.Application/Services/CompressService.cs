using FreeVideoCompressor.DataAccess.Repositories;
using FreeVideoCompressor.Domain.Models;
using FreeVideoCompressor.Domain.Utilities;

namespace FreeVideoCompressor.Application.Services;

public class CompressService
{
    private readonly CompressVideoFlowRepository _compressVideoFlowRepository;
    
    public CompressService(CompressVideoFlowRepository compressVideoFlowRepository)
    {
        _compressVideoFlowRepository = compressVideoFlowRepository;
    }
    
    public async Task<Result<Unit, string>> InitFlow(string filePath)
    {
        CompressVideoFlow compressVideoFlow = new(
            Id: Guid.NewGuid(),
            Status: CompressVideoFlowStatus.Uploaded,
            InputFilePath: filePath
        );
        
        Result<Unit, string> createResult = await _compressVideoFlowRepository.CreateAsync(compressVideoFlow);
        if (createResult.IsErr)
        {
            return Result<Unit, string>.Err($"Failed to create compress flow: {createResult.UnwrapErr()}");
        }
        
        return Result<Unit, string>.Ok(Unit.Value);
    }
}