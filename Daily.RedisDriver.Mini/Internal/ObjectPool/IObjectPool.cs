namespace Daily.RedisDriver.Mini.Internal.ObjectPool;

public interface IObjectPool<T> : IDisposable
{
    IPolicy<T> Policy { get; }


    /// <summary>
    /// 获取资源
    /// </summary>
    /// <param name="timeout">超时</param>
    /// <returns></returns>
    ObjectWarp<T> Get(TimeSpan? timeout = null);

    /// <summary>
    /// 使用完毕后，归还资源
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="isReset">是否重新创建</param>
    void Return(ObjectWarp<T> obj, bool isReset = false);


    /// <summary>
    /// 统计对象池中的对象
    /// </summary>
    string Statistics { get; }
}