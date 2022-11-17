namespace Daily.RedisDriver.Mini.Internal.ObjectPool;

public class DefaultPolicy<T> : IPolicy<T>
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    public int PoolSize { get; set; } = 10;

    public Func<T> CreateObject;

    public T OnCreate()
    {
        Name = Guid.NewGuid().ToString();
        return CreateObject.Invoke();
    }

    public void OnDestroy(T obj)
    {
    }

    public void OnGet(ObjectWarp<T> obj)
    {
    }

    public void OnReturn(ObjectWarp<T> obj)
    {
    }
}