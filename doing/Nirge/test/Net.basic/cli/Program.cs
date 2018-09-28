using System;
using log4net;
using log4net.Config;
using System.IO;
using System.Threading;
using Nirge.Core;
using System.Net;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace cli
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure(LogManager.CreateRepository("cli"), new FileInfo("../../Net.basic.log.cli.xml"));
            var cache = new CTcpClientCache(new CTcpClientCacheArgs(8192, 1073741824, 1073741824));

            const int gCapacity = 300;
            const int gPkg = 32;

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
                            cache.AllocSendBuf(rng.Next(1, gPkg << 1), out buf);
                            pkgs[i].Enqueue(buf);
                            cli.Send(pkgs[i]);
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}
