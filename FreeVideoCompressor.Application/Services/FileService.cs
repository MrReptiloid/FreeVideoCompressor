using FreeVideoCompressor.Domain.Abstractions;
using FreeVideoCompressor.Domain.Failures;
using FreeVideoCompressor.Domain.Utilities;

namespace FreeVideoCompressor.Application.Services;

public class FileService : IFileService
{
    public async Task<Result<byte[], FileFailure>> ReadFileAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return Result<byte[], FileFailure>.Err(
                FileFailure.InvalidPath("File path cannot be null or empty."));
        }

        try
        {
            if (!File.Exists(filePath))
            {
                return Result<byte[], FileFailure>.Err(FileFailure.InvalidPath());
            }

            byte[] bytes = await File.ReadAllBytesAsync(filePath);
            return Result<byte[], FileFailure>.Ok(bytes);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Result<byte[], FileFailure>.Err(FileFailure.AccessDenied(innerException: ex));
        }
        catch (Exception ex)
        {
            return Result<byte[], FileFailure>.Err(FileFailure.Unknown(innerException: ex));
        }
    }

    public async Task<Result<string, FileFailure>> SaveFileAsync(byte[] fileContent, string directory, string filename)
    {
        if (fileContent.Length == 0 || string.IsNullOrWhiteSpace(directory) || string.IsNullOrWhiteSpace(filename))
        {
            return Result<string, FileFailure>.Err(FileFailure.InvalidPath());
        }

        try
        {
             Directory.CreateDirectory(directory);
             string fulPath = Path.Combine(directory, filename);
             
             await File.WriteAllBytesAsync(fulPath, fileContent);
             return Result<string, FileFailure>.Ok(fulPath);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Result<string, FileFailure>.Err(FileFailure.AccessDenied(innerException: ex));
        }
        catch (IOException ex)
        {
            return Result<string, FileFailure>.Err(FileFailure.AlreadyExists(innerException: ex));
        }
        catch (Exception ex)
        {
            return Result<string, FileFailure>.Err(FileFailure.Unknown(innerException: ex));
        }
    }

    public Result<bool, FileFailure> FileExists(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return Result<bool, FileFailure>.Err(
                FileFailure.InvalidPath("File path cannot be null or empty."));
        }

        try
        {
            bool exists = File.Exists(filePath);
            return Result<bool, FileFailure>.Ok(exists);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Result<bool, FileFailure>.Err(FileFailure.AccessDenied(innerException: ex));
        }
        catch (Exception ex)
        {
            return Result<bool, FileFailure>.Err(FileFailure.Unknown(innerException: ex));
        }
    }
        

    public async Task<Result<bool, FileFailure>> DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return Result<bool, FileFailure>.Err(FileFailure.InvalidPath("File path cannot be null or empty."));
        }
        
        try
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(filePath))
                {
                    return Result<bool, FileFailure>.Err(FileFailure.NotFound());
                }

                File.Delete(filePath);
                return Result<bool, FileFailure>.Ok(true);
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Result<bool, FileFailure>.Err(FileFailure.AccessDenied(innerException: ex));
        }
        catch (Exception ex)
        {
            return Result<bool, FileFailure>.Err(FileFailure.Unknown(innerException: ex));
        }
    }
}