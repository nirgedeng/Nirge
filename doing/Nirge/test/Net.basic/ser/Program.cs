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
        static void Main(string[] args)
        {
            //
            XmlConfigurator.Configure(LogManager.CreateRepository("ser"), new FileInfo("../../Net.basic.log.ser.xml"));
            var cache = new CTcpClientCache(new CTcpClientCacheArgs(104857600, 104857600), LogManager.Exists("ser", "all"));
            var fill = new CTcpClientPkgFill();
            fill.AddPkg(typeof(ArraySegment<byte>), (int)eTcpClientPkgType.ArraySegment, new CTcpClientArraySegment());
            var code = new CTcpClientProtobufCode();
            code.Collect(typeof(G2C_PULSE_GEMON).Assembly);
            fill.AddPkg(typeof(IMessage<>), (int)eTcpClientPkgType.Protobuf, new CTcpClientProtobuf(code));

            //
            var ser = new CTcpServer(new CTcpServerArgs(capacity: 10240), LogManager.Exists("ser", "all"), cache, fill);
            ser.Closed += Ser_Closed;
            ser.CliConnected += Ser_CliConnected;
            ser.CliClosed += Ser_CliClosed;
            ser.CliRecved += Ser_CliRecved;
            ser.Open(new IPEndPoint(IPAddress.Parse("192.168.31.156"), 9527));

            var t = Environment.TickCount;

            while (true)
            {
                ser.Exec();

                if (Environment.TickCount > t + 10000)
                {
                    t = Environment.TickCount;
                    Console.WriteLine(cache.Stat);
                }

                Thread.Sleep(1);
            }
        }

        private static void Ser_CliRecved(object arg1, int arg2, object pkg)
        {
            CTcpServer ser = (CTcpServer)arg1;

            ser.Send(arg2, pkg);
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
