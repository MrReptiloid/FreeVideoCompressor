using FreeVideoCompressor.Domain.Failures;
using FreeVideoCompressor.Domain.Utilities;

namespace FreeVideoCompressor.Domain.Abstractions;

public interface IFileService
{
    Task<Result<byte[], FileFailure>> ReadFileAsync(string path);
    Task<Result<string, FileFailure>> SaveFileAsync(byte[] content, string directory, string fileName);
    Result<bool, FileFailure> FileExists(string path);
    Task<Result<bool, FileFailure>> DeleteFileAsync(string path);
}