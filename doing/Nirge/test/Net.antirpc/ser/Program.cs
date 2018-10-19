using log4net;
using log4net.Config;
using System;
using System.IO;
using Nirge.Core;
using System.Net;
using System.Threading;
using Google.Protobuf;

namespace ser
{
    class Program
    {
        static CRpcCallStubProvider _stubs;

        async static void f(CARpcCaller caller, int channel)
        {
            try
            {
                caller.a(channel);
                caller.b(channel);
                caller.c(new cargs() { A = 1, B = 1, C = "c", }, channel);
                caller.d(new dargs() { A = 1, B = 1, C = "d", }, channel);
                await caller.e(channel);
                var fret = caller.f(channel);
                await fret;
                //Console.WriteLine($"fret {fret.Result}");
                await caller.g(new gargs() { A = 1, B = 1, C = "g", }, channel);
                var hret = caller.h(new hargs() { A = 1, B = 1, C = "h", }, channel);
                await hret;
                //Console.WriteLine($"hret {hret.Result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static void Main(string[] args)
        {
            //
            XmlConfigurator.Configure(LogManager.CreateRepository("ser"), new FileInfo("../../Net.antirpc.log.ser.xml"));
            var cache = new CTcpClientCache(new CTcpClientCacheArgs(104857600, 104857600), LogManager.Exists("ser", "all"));
            var fill = new CTcpClientPkgFill();
            fill.AddPkg(typeof(ArraySegment<byte>), (int)eTcpClientPkgType.ArraySegment, new CTcpClientArraySegment());
            var code = new CTcpClientProtobufCode();
            code.Collect(typeof(bret).Assembly);
            fill.AddPkg(typeof(IMessage<>), (int)eTcpClientPkgType.Protobuf, new CTcpClientProtobuf(code));

            //
            var ser = new CTcpServer(new CTcpServerArgs(capacity: 10240), LogManager.Exists("ser", "all"), cache, fill);
            ser.Closed += Ser_Closed;
            ser.CliConnected += Ser_CliConnected;
            ser.CliClosed += Ser_CliClosed;
            ser.CliRecved += Ser_CliRecved;
            ser.Open(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9527));

            //
            var rpcStream = new CRpcStream(new byte[8192]);
            var rpcTransfer = new CServerRpcTransfer(ser);
            _stubs = new CRpcCallStubProvider(new CRpcCallStubProviderArgs(10240), LogManager.Exists("ser", "all"));
            var aCaller = new CARpcCaller(new CRpcCallerArgs(TimeSpan.FromSeconds(8)), LogManager.Exists("ser", "all")
                , rpcStream, rpcTransfer, _stubs);

            var t = Environment.TickCount;
            while (true)
            {
                ser.Exec();
                _stubs.Exec();

                foreach (var i in ser.Clis)
                {
                    f(aCaller, i);
                }

                if (Environment.TickCount > t + 10000)
                {
                    t = Environment.TickCount;
                    Console.WriteLine(cache.Stat);
                    Console.WriteLine($"stubs {_stubs.Count}");
                    Console.WriteLine($"CallsCount {aCaller.CallsCount}");
                }

                Thread.Sleep(10);
            }
        }

        private static void Ser_CliRecved(object arg1, int arg2, object pkg)
        {
            CTcpServer ser = (CTcpServer)arg1;

            if (pkg is RpcCallRsp rsp)
                _stubs.Exec(rsp);
            else if (pkg is RpcCallExceptionRsp exceptionRsp)
                _stubs.Exec(exceptionRsp);
        }

        private static void Ser_CliClosed(object sender, CDataEventArgs<int, CTcpClientCloseArgs> e)
        {
        }

        private static void Ser_CliConnected(object sender, CDataEventArgs<int> e)
        {
        }

        private static void Ser_Closed(object sender, CDataEventArgs<CTcpServerCloseArgs> e)
        {
        }
    }
}
