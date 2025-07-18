using FreeVideoCompressor.Domain.Utilities;

namespace FreeVideoCompressor.Domain.Failures;


public sealed record FileFailure(
    FileFailureType Failure,
    string Message,
    Exception? InnerException = null)
    : FailureBase(Message, InnerException)
{
    public static FileFailure NotFound(string msg = FileFailureMessageKeys.NotFound, Exception? innerException = null)
    {
        return new FileFailure(FileFailureType.NotFound, msg, innerException);
    }

    public static FileFailure AccessDenied(string msg = FileFailureMessageKeys.AccessDenied, 
        Exception? innerException = null)
    {
        return new  FileFailure(FileFailureType.AccessDenied, msg, innerException);
    }

    public static FileFailure InvalidPath(string msg = FileFailureMessageKeys.InvalidPath,
        Exception? innerException = null)
    {
        return new FileFailure(FileFailureType.InvalidPath, msg, innerException);
    }
    
    public static FileFailure AlreadyExists(string msg = FileFailureMessageKeys.AlreadyExists,
        Exception? innerException = null)
    {
        return new FileFailure(FileFailureType.AlreadyExists, msg, innerException);
    }

    public static FileFailure Unknown(string msg = FileFailureMessageKeys.Unknown, Exception? innerException = null)
    {
        return new FileFailure(FileFailureType.Unknown, msg, innerException);
    } 
}