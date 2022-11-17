using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daily.RedisDriver.Mini
{
    public partial class RedisClient
    {
        /// <summary>
        /// 存在时删除 key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int Del(string key) => Convert.ToInt32(Call("DEL", key));

        /// <summary>
        /// 检查给定 key 是否存在。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key) => Convert.ToBoolean(Call("EXISTS", key));
    }
}