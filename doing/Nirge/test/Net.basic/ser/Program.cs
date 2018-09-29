using log4net;
using log4net.Config;
using System;
using System.IO;
using Nirge.Core;
using System.Net;
using System.Threading;

namespace ser
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure(LogManager.CreateRepository("ser"), new FileInfo("../../Net.basic.log.ser.xml"));

            var cache = new CTcpClientCache(new CTcpClientCacheArgs(8192, 1073741824, 1073741824), LogManager.Exists("ser", "all"));
            var fill = new CTcpClientPkgFill();
            fill.Register(typeof(byte[]), new CTcpClientArraySegment());
            fill.Register(typeof(ArraySegment<byte>), new CTcpClientArraySegment());
            var ser = new CTcpServer(new CTcpServerArgs(), LogManager.Exists("ser", "all"), cache, fill);
            ser.Closed += Ser_Closed;
            ser.CliConnected += Ser_CliConnected;
            ser.CliClosed += Ser_CliClosed;
            ser.CliRecved += Ser_CliRecved;
            ser.Open(new IPEndPoint(IPAddress.Any, 9527));

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

        private static void Ser_CliRecved(object arg1, int arg2, byte[] arg3, int arg4, int arg5)
        {
            //Console.WriteLine("cli {0} recv msg {1}", arg2, arg5);
        }

        private static void Ser_CliClosed(object sender, CDataEventArgs<int, CTcpClientCloseArgs> e)
        {
            Console.WriteLine("cli {0} closed {1} {2} {3}", e.Arg1, e.Arg2.Reason, e.Arg2.SocketError, e.Arg2.Exception == null ? "" : e.Arg2.Exception.ToString());
        }

        private static void Ser_CliConnected(object sender, CDataEventArgs<int> e)
        {
            Console.WriteLine(e.Arg1);
        }

        private static void Ser_Closed(object sender, CDataEventArgs<CTcpServerCloseArgs> e)
        {
            Console.WriteLine(e.Arg1.Reason);
        }
    }
}
