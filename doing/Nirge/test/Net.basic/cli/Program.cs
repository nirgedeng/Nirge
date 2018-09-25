using System;
using log4net;
using log4net.Config;
using System.IO;
using System.Threading;
using Nirge.Core;
using System.Net;
using System.Collections.Generic;

namespace cli
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure(LogManager.CreateRepository("cli"), new FileInfo("../../Net.basic.log.cli.xml"));
            var cache = new CTcpClientCache(new CTcpClientCacheArgs());

            var clis = new List<CTcpClient>(1024);
            for (var i = 0; i < clis.Capacity; ++i)
            {
                var cli = new CTcpClient(new CTcpClientArgs(), LogManager.Exists("cli", "all"), cache);
                clis.Add(cli);
                cli.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9527));
            }

            var rng = new Random();

            while (true)
            {
                foreach (var i in clis)
                {
                    i.Exec();
                    if (i.State == eTcpClientState.Connected)
                    {
                        byte[] buf;
                        cache.AllocSendBuf(rng.Next(1024), out buf);
                        i.Send(buf);
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}
