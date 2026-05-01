namespace TCTOS.Util;

public static class Extensions
{
    public static TData WaitAndGet<TData>(this Task<TData> task)
    {
        task.Wait();
        return task.Result;
    }
}