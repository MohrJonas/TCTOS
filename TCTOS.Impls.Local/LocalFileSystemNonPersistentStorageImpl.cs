using System.Text.Json;
using TCTOS.Abstractions;

namespace TCTOS.Impls.Local;

public sealed class LocalFileSystemNonPersistentStorageImpl(string nonPersistentStorageRoot) : INonPersistentStorage
{
    public void PutValue<TData>(string key, TData value)
    {
        File.WriteAllText(BuildPathForKey(key), JsonSerializer.Serialize(value));
    }

    public TData GetValue<TData>(string key)
    {
        var path = BuildPathForKey(key);
        var value = JsonSerializer.Deserialize<TData>(File.ReadAllText(path))!;
        File.Delete(path);
        return value;
    }

    public TData PeekValue<TData>(string key)
    {
        return JsonSerializer.Deserialize<TData>(File.ReadAllText(BuildPathForKey(key)))!;
    }

    public bool HasKey(string key)
    {
        return File.Exists(BuildPathForKey(key));
    }

    private static string SanitizeKeyForPath(string key)
    {
        return Path.GetInvalidFileNameChars()
            .Aggregate(key, (s, c) => s.Replace(c.ToString(), string.Empty));
    }

    private string BuildPathForKey(string key)
    {
        return Path.Combine(nonPersistentStorageRoot, SanitizeKeyForPath(key));
    }
}