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
            var cache = new CTcpClientCache(new CTcpClientCacheArgs(1073741824, 1073741824), LogManager.Exists("cli", "all"));
            var fill = new CTcpClientPkgFill();
            fill.Register(typeof(byte[]), (int)eTcpClientPkgType.ArraySegment, new CTcpClientArraySegment());
            fill.Register(typeof(ArraySegment<byte>), (int)eTcpClientPkgType.ArraySegment, new CTcpClientArraySegment());

            const int gCapacity = 2000;
            var pkg = new byte[1024];

            var clis = new CTcpClient[gCapacity];
            for (var i = 0; i < clis.Length; ++i)
            {
                var cli = new CTcpClient(new CTcpClientArgs(), LogManager.Exists("cli", "all"), cache, fill);
                cli.Connected += Cli_Connected;
                cli.Closed += Cli_Closed;
                cli.Recved += Cli_Recved;
                clis[i] = cli;
                cli.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9527));
            }

            var rng = new Random();
            var t = Environment.TickCount;

            while (true)
            {
                for (var i = 0; i < gCapacity; ++i)
                {
                    var cli = clis[i];

                    cli.Exec();
                    switch (cli.State)
                    {
                    case eTcpClientState.Connected:
                        try
                        {
                            cli.Send(new ArraySegment<byte>(pkg, 0, rng.Next(1, pkg.Length)));
                            cli.Send(new ArraySegment<byte>(pkg, 0, rng.Next(1, pkg.Length)));
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                        break;
                    }
                }

                if (Environment.TickCount > t + 10000)
                {
                    t = Environment.TickCount;
                    Console.WriteLine(cache.Stat);
                }

                Thread.Sleep(100);
            }
        }

        private static void Cli_Recved(object arg1, object pkg)
        {
        }

        private static void Cli_Closed(object sender, CDataEventArgs<CTcpClientCloseArgs> e)
        {
            Console.WriteLine("cli close {0} {1} {2}", e.Arg1.Reason, e.Arg1.SocketError, e.Arg1.Exception != null ? e.Arg1.Exception.ToString() : "");
        }

        private static void Cli_Connected(object sender, CDataEventArgs<CTcpClientConnectArgs> e)
        {
            Console.WriteLine("cli connect {0} {1} {2}", e.Arg1.Result, e.Arg1.SocketError, e.Arg1.Exception != null ? e.Arg1.Exception.ToString() : "");
        }
    }
}
