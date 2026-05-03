namespace TCTOS.Abstractions;

public interface INonPersistentStorage
{
    public void PutValue<TData>(string key, TData value);
    public TData GetValue<TData>(string key);
    public TData PeekValue<TData>(string key);
    public bool HasKey(string key);
}