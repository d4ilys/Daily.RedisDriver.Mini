using Daily.RedisDriver.Mini.Internal.RedisClientPool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static Daily.RedisDriver.Mini.Internal.RespHelper;

namespace Daily.RedisDriver.Mini.Internal
{
    internal class RedisInteractors
    {
        /// <summary>
        /// 拼接发送数据
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        internal static string GetResult(TcpClient client, params string[] cmd)
        {
            var sb = new StringBuilder();
            sb.Append($"*{cmd.Length}{Environment.NewLine}");
            foreach (var item in cmd)
            {
                var length = Encoding.UTF8.GetByteCount(item);
                sb.Append($"${length}{Environment.NewLine}{item}{Environment.NewLine}");
            }

            //获取NetWorkSteam
            var stream = client.GetStream();
            //Send
            stream?.Write(Encoding.UTF8.GetBytes(sb.ToString()));
            return GetResponseData(stream);
        }

        /// <summary>
        /// AUTH
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        internal static string Auth(TcpClient client, string password) => GetResult(client, "AUTH", password);

        /// <summary>
        /// PING
        /// </summary>
        /// <returns></returns>
        internal static string Ping(TcpClient client) => GetResult(client, "PING");

        /// <summary>
        /// 获取返回数据
        /// </summary>
        /// <returns></returns>
        internal static string GetResponseData(Stream stream)
        {
            var resp3Reader = new RespHelper.Resp3Reader(stream);
            var res = resp3Reader.ReadObject(Encoding.UTF8);
            return res.Value.ToString();
        }
    }
}