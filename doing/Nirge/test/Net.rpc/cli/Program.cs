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
        static CRpcCallStubProvider _stubs;

        async static void f(CARpcCaller caller)
        {
            caller.a();
            caller.b();
            caller.c(new cargs() { A = 1, B = 1, C = "c", });
            caller.d(new dargs() { A = 1, B = 1, C = "d", });
            await caller.e();
            var fret = caller.f();
            await fret;
            Console.WriteLine($"fret {fret}");
            await caller.g(new gargs() { A = 1, B = 1, C = "c", });
            var hret = caller.h(new hargs() { A = 1, B = 1, C = "d", });
            await hret;
            Console.WriteLine($"hret {hret}");
        }

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
            const int gCapacity = 1;
            var clis = new CTcpClient[gCapacity];
            for (var i = 0; i < clis.Length; ++i)
            {
                var cli = new CTcpClient(new CTcpClientArgs(), LogManager.Exists("cli", "all"), cache, fill);
                cli.Connected += Cli_Connected;
                cli.Closed += Cli_Closed;
                cli.Recved += (sender, e) =>
                {
                    Cli_Recved(sender, i, e);
                };
                clis[i] = cli;
                cli.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9527));
            }

            var rpcStream = new CRpcStream(new byte[8192]);
            _stubs = new CRpcCallStubProvider(new CRpcCallStubProviderArgs(), LogManager.Exists("cli", "all"));
            var transfers = new CClientRpcTransfer[gCapacity];
            for (var i = 0; i < clis.Length; ++i)
            {
                var transfer = new CClientRpcTransfer(clis[i]);
                transfers[i] = transfer;
            }
            var aCallers = new CARpcCaller[gCapacity];
            for (var i = 0; i < clis.Length; ++i)
            {
                var aCaller = new CARpcCaller(new CRpcCallerArgs(TimeSpan.FromSeconds(4)), LogManager.Exists("cli", "all"), rpcStream, transfers[i], _stubs);
                aCallers[i] = aCaller;
            }

            var t = Environment.TickCount;
            while (true)
            {
                for (var i = 0; i < clis.Length; ++i)
                {
                    var cli = clis[i];
                    var aCaller = aCallers[i];
                    cli.Exec();
                    switch (cli.State)
                    {
                    case eTcpClientState.Connected:
                        try
                        {
                            f(aCaller);
                            _stubs.Exec();
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

        private static void Cli_Recved(object arg1, int channel, object pkg)
        {
            if (pkg is RpcCallRsp rsp)
                _stubs.Exec(rsp);
            else if (pkg is RpcCallExceptionRsp exceptionRsp)
                _stubs.Exec(exceptionRsp);
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
