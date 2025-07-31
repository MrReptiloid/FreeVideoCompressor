using FreeVideoCompressor.Domain.Models;
using FreeVideoCompressor.Domain.Utilities;

namespace FreeVideoCompressor.DataAccess.Repositories;

public class CompressVideoFlowRepository
{
    private readonly FreeVideoCompressorDbContext _dbContext;
    
    public CompressVideoFlowRepository(FreeVideoCompressorDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
    public async Task<Result<Unit, string>> CreateAsync(CompressVideoFlow compressVideoFlow)
    {
        try
        {
            await _dbContext.CompressVideoFlows.AddAsync(compressVideoFlow);
            await _dbContext.SaveChangesAsync();
            return Result<Unit, string>.Ok(Unit.Value);
        }
        catch (Exception ex)
        {
            return Result<Unit, string>.Err($"Failed to create CompressVideoFlow: {ex.Message}");
        }
    }

    public async Task<Result<CompressVideoFlow, string>> ReadAsync(Guid flowId)
    {
        CompressVideoFlow? result = await _dbContext.CompressVideoFlows.FindAsync(flowId);
        if (result == null)
        {
            return Result<CompressVideoFlow, string>.Err("CompressVideoFlow not found");
        }
        return Result<CompressVideoFlow, string>.Ok(result);
    }

   public async Task<Result<Unit, string>> PatchStatusAsync(Guid id, CompressVideoFlowStatus status)
{
    try
    {
        var existingFlow = await _dbContext.CompressVideoFlows.FindAsync(id);
        if (existingFlow == null)
        {
            return Result<Unit, string>.Err("CompressVideoFlow not found");
        }

        existingFlow.Status = status;
        await _dbContext.SaveChangesAsync();
        return Result<Unit, string>.Ok(Unit.Value);
    }
    catch (Exception ex)
    {
        return Result<Unit, string>.Err($"Failed to patch status: {ex.Message}");
    }
}
}