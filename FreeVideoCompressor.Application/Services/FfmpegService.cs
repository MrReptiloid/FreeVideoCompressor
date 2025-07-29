using System.Diagnostics;
using System.Text;
using FreeVideoCompressor.Domain.Utilities;

namespace FreeVideoCompressor.Application.Services;

public class FfmpegService
{
    public async Task CompressAsync(
        string inputFilePath, 
        string outputFilePath, 
        CancellationToken cancellationToken = default)
    {
        string args = $"-y -i \"{inputFilePath}\" -vcodec libx264 -crf 28 \"{outputFilePath}\"";
        
        var startInfo = new ProcessStartInfo
        {
            FileName = Constants.FfmpegAlias,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        using Process process = new () { StartInfo = startInfo };

        var outputBuilder = new StringBuilder();
        process.OutputDataReceived += (_, e) => outputBuilder.AppendLine(e.Data);
        process.ErrorDataReceived += (_, e) => outputBuilder.AppendLine(e.Data);

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        
        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"FFmpeg failed:\n{outputBuilder}");
        }
    }
}