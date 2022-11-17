using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Daily.RedisDriver.Mini.Internal.ObjectPool;
using Daily.RedisDriver.Mini.Internal;
using Daily.RedisDriver.Mini.Internal.RedisClientPool;
using static Daily.RedisDriver.Mini.Internal.RespHelper;

namespace Daily.RedisDriver.Mini
{
    public partial class RedisClient
    {
        internal RedisClientPool _redisClientPool;

        public RedisClient(string ConnectionString)
        {
            //实例化的时候会连接Redis
            _redisClientPool = new RedisClientPool(ConnectionString);
        }

        /// <summary>
        /// 拼接发送数据
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string Call(params string[] cmd)
        {
            var objectWarp = _redisClientPool.Get();
            var client = objectWarp.Value.GetClient();
            var result = RedisInteractors.GetResult(client, cmd);
            _redisClientPool.Return(objectWarp);
            return result;
        }

  

    }
}