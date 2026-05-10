namespace TCTOS.Daemon;

public sealed class DiContainer
{
    private readonly Dictionary<Type, ContainerElement> _backingDict = [];

    public TData Get<TData>()
    {
        return (TData)_backingDict[typeof(TData)].Get();
    }

    public DiContainer AddLazy<TData>(Func<TData> lazyFactory) where TData : notnull
    {
        _backingDict[typeof(TData)] = new LazyContainerElement(() => lazyFactory());
        return this;
    }

    public DiContainer Add<TData>(TData data) where TData : notnull
    {
        _backingDict[typeof(TData)] = new FixedContainerElement(data);
        return this;
    }

    private abstract class ContainerElement
    {
        public abstract object Get();
    }

    private sealed class FixedContainerElement(object element) : ContainerElement
    {
        public override object Get()
        {
            return element;
        }
    }

    private sealed class LazyContainerElement(Func<object> factory) : ContainerElement
    {
        private object? _value;

        public override object Get()
        {
            _value ??= factory();
            return _value;
        }
    }
}