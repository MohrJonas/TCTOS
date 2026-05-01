using System.Text.Json;
using TCTOS.Abstractions;

namespace TCTOS.Impls;

public sealed class LocalFileSystemNonPersistentStorageImpl : INonPersistentStorage
{
    private static string SanitizeKeyForPath(string key)
        => Path.GetInvalidFileNameChars()
            .Aggregate(key, (s, c) => s.Replace(c.ToString(), string.Empty));
    
    private static string BuildPathForKey(string key)
        => Path.Combine(Path.GetTempPath(), SanitizeKeyForPath(key));
    
    public void PutValue<TData>(string key, TData value) 
        => File.WriteAllText(BuildPathForKey(key), JsonSerializer.Serialize(value));

    public TData GetValue<TData>(string key)
        => JsonSerializer.Deserialize<TData>(File.ReadAllText(BuildPathForKey(key)))!;

    public bool HasKey(string key)
        => File.Exists(BuildPathForKey(key));
}