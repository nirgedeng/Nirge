/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Text;
using Nirge.Core;
using System.Net;
using log4net;
using System;

namespace ser
{
    class CSer
    {
        ILog _log;
        CTasker _task;
        CTaskTimer _timer;
        CTicker _tick;
        CTcpServer _ser;

        public void Init()
        {
            _log = LogManager.Exists("all");

            _task = new CTasker(_log);
            _timer = new CTaskTimer(_task, _log);
            _tick = new CTicker();

            _ser = new CTcpServer(_log);
            _ser.Closed += Ser_Closed;
            _ser.CliConnected += Ser_CliConnected;
            _ser.CliClosed += Ser_CliClosed;
            _ser.CliRecved += Ser_CliRecved;

            _task.Exec(CCall.Create(() =>
            {
                _ser.Open(new IPEndPoint(IPAddress.Any, 9527));
            }));
            _timer.Reg(CCall.Create(() =>
            {
                for (int i = 0; i < 8; ++i)
                    _ser.Exec();
            }), 10);
            _tick.Ticked += (sender, e) =>
            {
                _timer.Exec(e);
            };

            _task.Init();
            _timer.Init();
            _tick.Init();
        }

        public void Destroy()
        {
            _ser.Close();
            _tick.Destroy();
            _timer.Destroy();
            _task.Destroy();
        }

        private void Ser_Closed(object sender, CDataEventArgs<CTcpServerCloseArgs> e)
        {
        }

        private void Ser_CliRecved(object sender, int cli, byte[] arg2, int arg3, int arg4)
        {
            _ser.Send(cli, arg2, arg3, arg4);
        }

        private void Ser_CliClosed(object sender, CDataEventArgs<int, CTcpClientCloseArgs> e)
        {
            var cli = e.Arg1;

            _log.InfoFormat("OnClosed, {0},{1},{2},{3}", cli, e.Arg2.Reason, e.Arg2.Error, e.Arg2.SocketError);
        }

        private void Ser_CliConnected(object sender, CDataEventArgs<int> e)
        {
            var cli = e.Arg1;

            _log.InfoFormat("OnConnected, {0}", cli);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var ser = new CSer();
            ser.Init();
            Console.ReadKey();
            ser.Destroy();
        }
    }
}
