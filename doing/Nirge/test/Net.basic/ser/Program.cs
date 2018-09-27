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

            var ser = new CTcpServer(LogManager.Exists("ser", "all"));
            ser.Closed += Ser_Closed;
            ser.CliConnected += Ser_CliConnected;
            ser.CliClosed += Ser_CliClosed;
            ser.CliRecved += Ser_CliRecved;
            ser.Open(new IPEndPoint(IPAddress.Any, 9527));

            while (true)
            {
                ser.Exec();
                Thread.Sleep(1);
            }

        }

        private static void Ser_CliRecved(object arg1, int arg2, byte[] arg3, int arg4, int arg5)
        {
            Console.WriteLine("cli {0} recv msg {1}", arg2, arg5);
        }

        private static void Ser_CliClosed(object sender, CDataEventArgs<int, CTcpClientCloseArgs> e)
        {
            Console.WriteLine("cli {0} closed {1} {2} {3}", e.Arg1, e.Arg2.Reason, e.Arg2.Error, e.Arg2.SocketError);
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
