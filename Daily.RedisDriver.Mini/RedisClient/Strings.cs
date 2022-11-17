using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daily.RedisDriver.Mini
{
    public partial class RedisClient
    {
        public string Get(string key) => Call("GET", key);

        public string Set(string key, string value) => Call("SET", key, value);

    }
}