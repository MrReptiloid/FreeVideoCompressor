namespace FreeVideoCompressor.Domain.Failures;

public enum FileFailureType
{
    NotFound,
    AccessDenied,
    InvalidPath,
    AlreadyExists,
    UnsupportedFormat,
    Unknown
}