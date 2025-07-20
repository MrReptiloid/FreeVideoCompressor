using FreeVideoCompressor.Domain.Utilities;

namespace FreeVideoCompressor.Domain.Failures;


public sealed record FileFailure(
    FileFailureType Failure,
    string BaseMessage,
    string? Message,
    Exception? InnerException = null)
    : FailureBase(BaseMessage, Message, InnerException)
{
    public static FileFailure NotFound(string? msg = null, Exception? innerException = null)
    {
        return new FileFailure(FileFailureType.NotFound, FileFailureMessageKeys.NotFound, msg, innerException);
    }

    public static FileFailure AccessDenied(string? msg = null, Exception? innerException = null)
    {
        return new FileFailure(FileFailureType.AccessDenied, FileFailureMessageKeys.AccessDenied, msg, innerException);
    }

    public static FileFailure InvalidPath(string? msg = null, Exception? innerException = null)
    {
        return new FileFailure(FileFailureType.InvalidPath, FileFailureMessageKeys.InvalidPath, msg, innerException);
    }
    
    public static FileFailure AlreadyExists(string? msg = null, Exception? innerException = null)
    {
        return new FileFailure(FileFailureType.AlreadyExists, FileFailureMessageKeys.AlreadyExists, msg,
            innerException);
    }

    public static FileFailure UnsupportedFormat(string? msg = null, Exception? innerException = null)
    {
        return new FileFailure(FileFailureType.UnsupportedFormat, FileFailureMessageKeys.UnsupportedFormat, msg,
            innerException);
    }

    public static FileFailure Unknown(string? msg = null, Exception? innerException = null)
    {
        return new FileFailure(FileFailureType.Unknown, FileFailureMessageKeys.Unknown, msg, innerException);
    } 
}