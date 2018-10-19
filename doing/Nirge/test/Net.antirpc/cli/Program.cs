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
using System.Linq;

namespace cli
{
    class Program
    {
        static CARpcCallee[] aCallees;

        static void Main(string[] args)
        {
            //
            XmlConfigurator.Configure(LogManager.CreateRepository("cli"), new FileInfo("../../Net.antirpc.log.cli.xml"));
            var cache = new CTcpClientCache(new CTcpClientCacheArgs(104857600, 104857600), LogManager.Exists("cli", "all"));
            var fill = new CTcpClientPkgFill();
            fill.AddPkg(typeof(ArraySegment<byte>), (int)eTcpClientPkgType.ArraySegment, new CTcpClientArraySegment());
            var code = new CTcpClientProtobufCode();
            code.Collect(typeof(bret).Assembly);
            fill.AddPkg(typeof(IMessage<>), (int)eTcpClientPkgType.Protobuf, new CTcpClientProtobuf(code));

            //
            const int gCapacity = 1;
            var clis = new CTcpClient[gCapacity];
            for (var i = 0; i < clis.Length; ++i)
            {
                var channel = i;
                var cli = new CTcpClient(new CTcpClientArgs(), LogManager.Exists("cli", "all"), cache, fill);
                cli.Connected += Cli_Connected;
                cli.Closed += Cli_Closed;
                cli.Recved += (sender, e) =>
                {
                    Cli_Recved(sender, channel, e);
                };
                clis[i] = cli;
                cli.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9527));
            }

            //
            var rpcStream = new CRpcStream(new byte[8192]);
            var aService = new CARpcService();
            var transfers = new CClientRpcTransfer[gCapacity];
            for (var i = 0; i < clis.Length; ++i)
            {
                var transfer = new CClientRpcTransfer(clis[i]);
                transfers[i] = transfer;
            }
            aCallees = new CARpcCallee[gCapacity];
            for (var i = 0; i < clis.Length; ++i)
            {
                var aCaller = new CARpcCallee(new CRpcCalleeArgs(), LogManager.Exists("cli", "all"), rpcStream, transfers[i], aService);
                aCallees[i] = aCaller;
            }

            var t = Environment.TickCount;
            while (true)
            {
                for (var i = 0; i < clis.Length; ++i)
                {
                    var cli = clis[i];
                    cli.Exec();
                    switch (cli.State)
                    {
                    case eTcpClientState.Connected:
                        try
                        {
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
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
                Thread.Sleep(10);
            }
        }

        private static void Cli_Recved(object arg1, int channel, object pkg)
        {
            CTcpClient cli = (CTcpClient)arg1;
            var aCallee = (CARpcCallee)aCallees[channel];

            if (pkg is RpcCallReq req)
                aCallee.Call(channel, req);
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
