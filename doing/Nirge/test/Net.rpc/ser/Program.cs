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
        static CTcpServer _ser;
        static CARpcCallee _aCallee;

        static void Main(string[] args)
        {
            //
            XmlConfigurator.Configure(LogManager.CreateRepository("ser"), new FileInfo("../../Net.rpc.log.ser.xml"));
            var cache = new CTcpClientCache(new CTcpClientCacheArgs(104857600, 104857600), LogManager.Exists("ser", "all"));
            var fill = new CTcpClientPkgFill();
            fill.Register(typeof(ArraySegment<byte>), (int)eTcpClientPkgType.ArraySegment, new CTcpClientArraySegment());
            var code = new CTcpClientProtobufCode();
            code.Collect(typeof(bret).Assembly);
            fill.Register(typeof(IMessage<>), (int)eTcpClientPkgType.Protobuf, new CTcpClientProtobuf(code));

            //
            _ser = new CTcpServer(new CTcpServerArgs(capacity: 10240), LogManager.Exists("ser", "all"), cache, fill);
            _ser.Closed += Ser_Closed;
            _ser.CliConnected += Ser_CliConnected;
            _ser.CliClosed += Ser_CliClosed;
            _ser.CliRecved += Ser_CliRecved;
            _ser.Open(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9527));

            //
            var rpcStream = new CRpcStream(new byte[8192]);
            var rpcTransfer = new CServerRpcTransfer(_ser);
            var aService = new CARpcService();
            _aCallee = new CARpcCallee(new CRpcCalleeArgs(), LogManager.Exists("ser", "all")
                , rpcStream, rpcTransfer, aService);

            var t = Environment.TickCount;
            while (true)
            {
                _ser.Exec();

                if (Environment.TickCount > t + 10000)
                {
                    t = Environment.TickCount;
                    Console.WriteLine(cache.Stat);
                }

                Thread.Sleep(100);
            }
        }

        private static void Ser_CliRecved(object arg1, int arg2, object pkg)
        {
            CTcpServer ser = (CTcpServer)arg1;

            if (pkg is RpcCallReq e)
                _aCallee.Call(arg2, e);
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
