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

            const int gCapacity = 10;
            const int gPkg = 128;

            var clis = new CTcpClient[gCapacity];
            for (var i = 0; i < clis.Length; ++i)
            {
                var cli = new CTcpClient(new CTcpClientArgs(), LogManager.Exists("cli", "all"), cache);
                cli.Connected += Cli_Connected;
                cli.Closed += Cli_Closed;
                cli.Recved += Cli_Recved;
                clis[i] = cli;
                cli.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9527));
            }

            var pkgs = new Queue<ArraySegment<byte>>[gCapacity];
            for (var i = 0; i < clis.Length; ++i)
            {
                pkgs[i] = new Queue<ArraySegment<byte>>();
            }

            var rng = new Random();
            var t = Environment.TickCount;

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

                if (Environment.TickCount > t + 10000)
                {
                    t = Environment.TickCount;
                    Console.WriteLine(cache.Stat);
                }

                Thread.Sleep(1);
            }
        }

        private static void Cli_Recved(object arg1, byte[] arg2, int arg3, int arg4)
        {
        }

        private static void Cli_Closed(object sender, CDataEventArgs<CTcpClientCloseArgs> e)
        {
            Console.WriteLine("cli close {0} {1} {2} {3}", e.Arg1.Reason, e.Arg1.Error, e.Arg1.SocketError, e.Arg1.Exception != null ? e.Arg1.Exception.ToString() : "");
        }

        private static void Cli_Connected(object sender, CDataEventArgs<CTcpClientConnectArgs> e)
        {
            Console.WriteLine("cli connect {0} {1} {2} {3}", e.Arg1.Result, e.Arg1.Error, e.Arg1.SocketError, e.Arg1.Exception != null ? e.Arg1.Exception.ToString() : "");
        }
    }
}
