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
        static void f(IEnumerable<int> v)
        {
        }

        static void Main(string[] args)
        {
            XmlConfigurator.Configure(LogManager.CreateRepository("cli"), new FileInfo("../../Net.basic.log.cli.xml"));
            var cache = new CTcpClientCache(new CTcpClientCacheArgs());

            const int gCapacity = 128;

            var clis = new CTcpClient[gCapacity];
            for (var i = 0; i < clis.Length; ++i)
            {
                var cli = new CTcpClient(new CTcpClientArgs(), LogManager.Exists("cli", "all"), cache);
                clis[i] = cli;
                cli.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9527));
            }

            var pkgs = new Queue<ArraySegment<byte>>[gCapacity];
            for (var i = 0; i < clis.Length; ++i)
            {
                pkgs[i] = new Queue<ArraySegment<byte>>();
            }

            var rng = new Random();

            while (true)
            {
                for (var i = 0; i < gCapacity; ++i)
                {
                    var cli = clis[i];

                    cli.Exec();
                    if (cli.State == eTcpClientState.Connected)
                    {
                        if (cache.CanAllocSendBuf)
                        {
                            byte[] buf;
                            var result = cache.AllocSendBuf(rng.Next(1024), out buf);
                            switch (result)
                            {
                            case eTcpError.Success:
                                pkgs[i].Enqueue(buf);
                                cli.Send(pkgs[i]);
                                break;
                            }
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}
