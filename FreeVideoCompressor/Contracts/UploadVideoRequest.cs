using System.ComponentModel.DataAnnotations;

namespace FreeVideoCompressor.Contracts;

public sealed record UploadVideoRequest
(
    [Required]
    IFormFile VideoFile
);