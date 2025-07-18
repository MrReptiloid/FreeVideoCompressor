using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FreeVideoCompressor.Domain.Contracts;

public sealed record UploadVideoRequest(
    [Required] IFormFile VideoFile
);