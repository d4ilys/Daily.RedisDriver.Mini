namespace Daily.RedisDriver.Mini.Internal.ObjectPool;

/// <summary>
/// 对象池创建的规则
/// </summary>
public interface IPolicy<T>
{
    /// <summary>
    /// 名称
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// 对象池大小
    /// </summary>
    int PoolSize { get; set; }

    /// <summary>
    /// 对象池的对象被创建时
    /// </summary>
    /// <returns>返回被创建的对象</returns>
    T OnCreate();

    /// <summary>
    /// 销毁对象
    /// </summary>
    /// <param name="obj">资源对象</param>
    void OnDestroy(T obj);


    /// <summary>
    /// 从对象池获取对象成功的时候触发，通过该方法统计或初始化对象
    /// </summary>
    /// <param name="obj">资源对象</param>
    void OnGet(ObjectWarp<T> obj);


    /// <summary>
    /// 归还对象给对象池的时候触发
    /// </summary>
    /// <param name="obj">资源对象</param>
    void OnReturn(ObjectWarp<T> obj);
}