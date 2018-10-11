using System;
using log4net;
using log4net.Config;
using System.IO;
using System.Threading;
using Nirge.Core;
using System.Net;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Google.Protobuf;

namespace cli
{
    class Program
    {
        static void Main(string[] args)
        {
            //
            XmlConfigurator.Configure(LogManager.CreateRepository("cli"), new FileInfo("../../Net.rpc.log.cli.xml"));
            var cache = new CTcpClientCache(new CTcpClientCacheArgs(104857600, 104857600), LogManager.Exists("cli", "all"));
            var fill = new CTcpClientPkgFill();
            fill.Register(typeof(ArraySegment<byte>), (int)eTcpClientPkgType.ArraySegment, new CTcpClientArraySegment());
            var code = new CTcpClientProtobufCode();
            code.Collect(typeof(bret).Assembly);
            fill.Register(typeof(IMessage<>), (int)eTcpClientPkgType.Protobuf, new CTcpClientProtobuf(code));

            //
            const int gCapacity = 1000;
            var t = Environment.TickCount;

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

            while (true)
            {
                foreach (var i in clis)
                {
                    i.Exec();
                    switch (i.State)
                    {
                    case eTcpClientState.Connected:
                        try
                        {
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

                //
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
