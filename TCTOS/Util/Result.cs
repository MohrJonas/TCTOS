using System;
using System.Threading.Tasks;

namespace TCTOS.Util;

public class Result(bool hasFailed, Exception? exception)
{
    public override string ToString() 
        => $"{(hasFailed ? "Failed" : "Succeeded")}: {(hasFailed ? exception : string.Empty)}";

    public void ThrowIfFailed()
    {
        if (hasFailed)
            throw exception!;
    }

    public bool HasFailed => hasFailed;

    public Exception? Exception => exception;
}

public sealed class Result<TData>(bool hasFailed, Exception? exception, TData? data) : Result(hasFailed, exception)
{
    private readonly TData? _data = data;

    public TData GetOrThrow() => HasFailed ? throw Exception! : _data!;
    
    public override string ToString() 
        => $"{(HasFailed ? "Failed" : "Succeeded")}: {(HasFailed ? Exception : _data)}";
}

public static class ResultStatics
{
    public static Result RunCatching(Action action)
    {
        try
        {
            action();
            return new Result(false, null);
        }
        catch (Exception e)
        {
            return new Result(true, e);
        }
    }

    public static Result<TData> RunCatching<TData>(Func<TData> func) where TData : notnull
    {
        try
        {
            return new Result<TData>(false, null, func());
        }
        catch (Exception e)
        {
            return new Result<TData>(true, e, default);
        }
    }
    
    public static async Task<Result> RunCatchingAsync(Func<Task> func)
    {
        try
        {
            await func();
            return new Result(false, null);
        }
        catch (Exception e)
        {
            return new Result(true, e);
        }
    }

    public static async Task<Result<TData>> RunCatchingAsync<TData>(Func<Task<TData>> func)
    {
        try
        {
            return new Result<TData>(false, null, await func());
        }
        catch (Exception e)
        {
            return new Result<TData>(true, e, default);
        }
    }
}