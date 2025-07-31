namespace FreeVideoCompressor.Domain.Utilities;

public static class ResultExtensions
{
    public static async Task<Result<TNext, TE>> BindAsync<T, TNext, TE>(
        this Task<Result<T, TE>> task,
        Func<T, Task<Result<TNext, TE>>> bindFn)
    {
        Result<T, TE> result = await task;
        return result.IsOk
            ? await bindFn(result.Unwrap())
            : Result<TNext, TE>.Err(result.UnwrapErr());
    }

    public static async Task<Result<TNext, TE>> Bind<T, TNext, TE>(
        this Task<Result<T, TE>> task,
        Func<T, Result<TNext, TE>> bindFn)
    {
        Result<T, TE> result = await task;
        return result.IsOk
            ? bindFn(result.Unwrap())
            : Result<TNext, TE>.Err(result.UnwrapErr());
    }
    
    public static async Task<Result<TNext, TE>> Map<T, TNext, TE>(
        this Task<Result<T, TE>> task,
        Func<T, TNext> mapFn)
    {
        Result<T, TE> result = await task;

        return result.IsOk
            ? Result<TNext, TE>.Ok(mapFn(result.Unwrap()))
            : Result<TNext, TE>.Err(result.UnwrapErr());
    }

    public static async Task<Result<T, TENext>> MapErr<T, TError, TENext>(
        this Task<Result<T, TError>> task,
        Func<TError, TENext> mapFn)
        where TENext : notnull
    {
        var result = await task;

        return result.IsOk
            ? Result<T, TENext>.Ok(result.Unwrap())
            : Result<T, TENext>.Err(mapFn(result.UnwrapErr()));
    }
    
}