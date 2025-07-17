using Microsoft.AspNetCore.Mvc;

namespace FreeVideoCompressor.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloWorldController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> HelloWorld()
    {
        return Ok(new { Message = "Hello World!"})
;    }
}