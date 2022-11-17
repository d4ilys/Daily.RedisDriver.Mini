using System.Collections.Concurrent;
using System.Runtime;
using System.Threading;

namespace Daily.RedisDriver.Mini.Internal.ObjectPool;

public partial class ObjectPool<T> : IObjectPool<T>
{
    public ObjectPool(int poolSize, Func<T> createObjecDelegate) : this(new DefaultPolicy<T>()
        { CreateObject = createObjecDelegate, PoolSize = poolSize })
    {
    }

    public ObjectPool(IPolicy<T> policy)
    {
        Policy = policy;
    }

    public IPolicy<T> Policy { get; protected set; }


    public string Statistics => $"Pool: {_freeObjects.Count}/{_allObjects.Count} 排队: {_getSyncQueue.Count}";

    private ConcurrentQueue<GetSyncQueueInfo> _getSyncQueue = new();

    private ConcurrentQueue<bool> _getQueue = new ConcurrentQueue<bool>();

    //线程安全的栈
    internal protected ConcurrentStack<ObjectWarp<T>> _freeObjects = new();

    internal protected List<ObjectWarp<T>> _allObjects = new();

    private object _allObjectsLock = new object();

    private ObjectWarp<T> getFree()
    {
        //在栈中直接拿到对象
        var isPop = _freeObjects.TryPop(out var obj);
        //如果在栈那不到对象，并且对象集合中对象数量 小于设置的规则中的数量才会去创建对象
        if (isPop == false && obj == null && _allObjects.Count < Policy.PoolSize)
        {
            lock (_allObjectsLock)
            {
                if (_allObjects.Count < Policy.PoolSize)
                    _allObjects.Add(obj = new ObjectWarp<T>()
                    {
                        Pool = this
                    });
            }
        }

        if (obj != null)
            obj._isReturned = false;

        if (obj != null && obj.Value == null)
        {
            try
            {
                obj._isReturned = false;
                obj.ResetValue();
            }
            catch
            {
                Return(obj);
                throw;
            }
        }

        return obj;
    }

    public ObjectWarp<T> Get(TimeSpan? timeout = null)
    {
        var obj = getFree();

        //如果对象池中的对象已经用尽了
        if (obj == null)
        {
            //直接把等待的对象放入队列
            var queueItem = new GetSyncQueueInfo();
            _getSyncQueue.Enqueue(queueItem);
            _getQueue.Enqueue(false);
            try
            {
                //放入队列后这个对象直接等待，如果Return的时候Set以后放行 
                queueItem.Wait.Wait();
                //Return-Set
                obj = queueItem.ReturnValue;
             
            }
            catch
            {
            }
        }

        return obj;
    }

    public void Return(ObjectWarp<T> obj, bool isReset = false)
    {
        if (obj == null)
            return;
        if (obj._isReturned)
            return;
        if (isReset)
            obj.ResetValue();

        bool isReturn = false;

        //每次归还对象的时候查看是否有没有排队的线程
        while (isReturn == false && _getQueue.TryDequeue(out var isAsync))
        {
            if (isAsync == false)
            {
                if (_getSyncQueue.TryDequeue(out var queueItem) && queueItem != null)
                {
                    lock (queueItem.Lock)
                    {
                        queueItem.ReturnValue = obj;
                    }

                    if (queueItem.ReturnValue != null)
                    {
                        try
                        {
                            queueItem.Wait.Set();

                            //用完直接归还
                            isReturn = true;
                        }
                        catch
                        {
                        }
                    }

                    try
                    {
                        queueItem.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
        }

        //无排队，直接归还
        if (isReturn == false)
        {
            try
            {
                Policy.OnReturn(obj);
            }
            catch
            {
                throw;
            }
            finally
            {
                obj._isReturned = true;
                _freeObjects.Push(obj);
            }
        }
    }

    public void Dispose()
    {


        while (_freeObjects.TryPop(out var fo))
            ;

        while (_getSyncQueue.TryDequeue(out var sync))
        {
            try
            {
                sync.Wait.Set();
            }
            catch { }
        }

        while (_getQueue.TryDequeue(out var qs))
            ;

        for (var a = 0; a < _allObjects.Count; a++)
        {
            Policy.OnDestroy(_allObjects[a].Value);
            try
            {
                (_allObjects[a].Value as IDisposable)?.Dispose();
            }
            catch { }
        }

        _allObjects.Clear();
    }

    class GetSyncQueueInfo : IDisposable
    {
        internal ManualResetEventSlim Wait { get; set; } = new ManualResetEventSlim();

        internal ObjectWarp<T> ReturnValue { get; set; }

        internal object Lock = new object();

        internal bool IsTimeout { get; set; } = false;

        public void Dispose()
        {
            try
            {
                if (Wait != null)
                    Wait.Dispose();
            }
            catch
            {
            }
        }
    }
}