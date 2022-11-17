using Daily.RedisDriver.Mini.Internal.ObjectPool;

namespace Daily.RedisDriver.Mini.Internal.RedisClientPool
{
    internal class RedisClientPool: ObjectPool<RedisConnect>, IDisposable
    {
        internal RedisClientPoolPolicy _policy;
        public string Prefix => _policy._connectionStringBuilder.Prefix;
        public RedisClientPool(ConnectionStringBuilder connectionString, Action<RedisClient>? connected = null) : base(null)
        {
            _policy = new RedisClientPoolPolicy()
            {
                _pool = this
            };
            _policy.Connected += (sender, args) =>
            {
                var cli = sender as RedisClient;
            };
            this.Policy = _policy;
            //注意这里，赋值连接字符串触发连接池赋值动作
            _policy.ConnectionString = connectionString;
        }
    }
}