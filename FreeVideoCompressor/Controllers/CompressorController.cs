using FreeVideoCompressor.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FreeVideoCompressor.Controllers;

[ApiController]
[Route("[controller]")]
[RequestSizeLimit(500 * 1024 * 1024)] //500 MB limit of file size
public class CompressorController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CompressVideo([FromForm]UploadVideoRequest request)
    {
        if (request.VideoFile.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }
        
        return Ok(new { Message = "Video compression is not implemented yet." });
    }
}
