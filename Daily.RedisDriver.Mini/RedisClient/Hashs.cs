using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daily.RedisDriver.Mini
{
    public partial class RedisClient
    {
        public int HSet<T>(string key, string field, T value)
        {
            var r = Call("HSET", key, field, value.ToString());
            return Convert.ToInt32(r);
        }

        public string HGet(string key, string field) => Call("HGET", key, field);
    }
}