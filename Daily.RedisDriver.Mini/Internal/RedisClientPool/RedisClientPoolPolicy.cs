using System.Collections.Concurrent;
using System.Data.Common;
using System.Text.RegularExpressions;
using Daily.RedisDriver.Mini.Internal.ObjectPool;

namespace Daily.RedisDriver.Mini.Internal.RedisClientPool;

internal class RedisClientPoolPolicy : IPolicy<RedisConnect>
{
    internal RedisClientPool _pool;

    internal ConnectionStringBuilder _connectionStringBuilder = new ConnectionStringBuilder();

    internal string Key => $"{_connectionStringBuilder.Host}/{_connectionStringBuilder.Database}";

    public event EventHandler Connected;

    public int PoolSize
    {
        get => _connectionStringBuilder.MaxPoolSize;
        set => _connectionStringBuilder.MaxPoolSize = value;
    }

    public string ConnectionString
    {
        get => _connectionStringBuilder.ToString();
        set
        {
            _connectionStringBuilder = value;

            if (_connectionStringBuilder.MinPoolSize > 0)
                //这里预热来预热一波
                PrevReheatConnectionPool(_pool, _connectionStringBuilder);
        }
    }

    public string Name { get; set; }

    public RedisConnect OnCreate()
    {
        var client = new RedisConnect(_connectionStringBuilder.IP, _connectionStringBuilder.Port);
        return client;
    }

    public void OnDestroy(RedisConnect obj)
    {
        //if (obj != null)
        //{
        //    //if (obj.IsConnected) try { obj.Quit(); } catch { } 此行会导致，服务器主动断开后，执行该命令超时停留10-20秒
        //    try
        //    {
        //        obj.Dispose();
        //    }
        //    catch { }
        //}
    }

    public void OnGet(ObjectWarp<RedisConnect> obj)
    {
        throw new NotImplementedException();
    }

    public void OnReturn(ObjectWarp<RedisConnect> obj)
    {
    }

    public static void PrevReheatConnectionPool(ObjectPool<RedisConnect> pool,
        ConnectionStringBuilder connectionStringBuilder)
    {
        int minPoolSize = connectionStringBuilder.MinPoolSize;
        if (minPoolSize <= 0)
            minPoolSize = Math.Min(5, pool.Policy.PoolSize);
        if (minPoolSize > pool.Policy.PoolSize)
            minPoolSize = pool.Policy.PoolSize;

        var initTestOk = true;

        var initConns = new ConcurrentBag<ObjectWarp<RedisConnect>>();

        var initStartTime = DateTime.Now;

        try
        {
            var conn = pool.Get();
            RedisInteractors.Auth(conn.Value.GetClient(), connectionStringBuilder.Password);
            RedisInteractors.Ping(conn.Value.GetClient());
            initConns.Add(conn);
        }
        catch (Exception ex)
        {
            initTestOk = false; //预热一次失败，后面将不进行
            //pool.SetUnavailable(ex);
        }

        for (int i = 1; initTestOk && i < minPoolSize; i++)
        {
            if (initStartTime.Subtract(DateTime.Now).TotalSeconds > 3)
                break; //预热耗时超过3秒，退出
            var b = Math.Min(minPoolSize - i, 10); //每10个预热
            var initTasks = new Task[b];
            for (int j = 0; j < b; j++)
            {
                initTasks[j] = Task.Run(() =>
                {
                    try
                    {
                        var conn = pool.Get();
                        //预热验证密码
                        if (!string.IsNullOrEmpty(connectionStringBuilder.Password))
                        {
                            RedisInteractors.Auth(conn.Value.GetClient(), connectionStringBuilder.Password);
                        }

                        initConns.Add(conn);
                    }
                    catch
                    {
                        initTestOk = false; //有失败，下一组退出预热
                    }
                });
            }

            Task.WaitAll(initTasks);
        }

        while (initConns.TryTake(out var conn))
        {
            pool.Return(conn);
        }
    }
}