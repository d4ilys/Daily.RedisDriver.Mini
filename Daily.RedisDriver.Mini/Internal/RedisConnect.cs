using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Daily.RedisDriver.Mini.Internal
{
    public class RedisConnect
    {
        internal TcpClient? TcpClient { get; set; }

        private readonly object _lock = new object();

        private string _host;
        private int _port;

        public RedisConnect(string host,int port)
        {
            _host = host;
            _port = port;
            Connection();
        }

        public TcpClient GetClient() => TcpClient;

        public bool IsConnect { get; set; } = false;

        public void Connection()
        {
            lock (_lock)
            {
                TcpClient = new TcpClient();
                TcpClient.Connect(_host, _port);
                IsConnect = TcpClient.Connected;
            }
        }
    }
}