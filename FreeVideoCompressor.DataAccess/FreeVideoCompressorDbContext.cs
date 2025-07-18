using FreeVideoCompressor.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FreeVideoCompressor.DataAccess;

public class FreeVideoCompressorDbContext : DbContext
{
    public FreeVideoCompressorDbContext(DbContextOptions<FreeVideoCompressorDbContext> options)
        : base(options)
    {
        
    }
    
    public DbSet<CompressVideoFlow> CompressVideoFlows { get; set; }
}