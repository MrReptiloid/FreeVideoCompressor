namespace FreeVideoCompressor.Domain.Failures;

public static class FileFailureMessageKeys
{
    public const string NotFound = "file_error_not_found";
    public const string AccessDenied = "file_error_access_denied";
    public const string InvalidPath = "file_error_invalid_path";
    public const string AlreadyExists = "file_error_already_exists";
    public const string UnsupportedFormat = "file_error_unsupported_format";
    public const string TooLarge = "file_error_too_large";
    public const string Unknown = "file_error_unknown";
}