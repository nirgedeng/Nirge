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

namespace cli
{
    class CCli
    {
        ILog _log;
        CTasker _task;
        CTaskTimer _timer;
        CTicker _tick;

        TcpClientCache _cache;
        CTcpClient _cli;

        public void Init()
        {
            _log = LogManager.Exists("all");

            _task = new CTasker(_log);
            _timer = new CTaskTimer(_task, _log);
            _tick = new CTicker();

            _cache = new TcpClientCache(new TcpClientCacheArgs(25600, 12800, 6400, 25600, 12800, 6400));
            _cli = new CTcpClient(new CTcpClientArgs(), _log, _cache);

            _task.Exec(CCall.Create(() =>
            {
                _cli.Connected += OnConnected;
                _cli.Closed += OnClosed;
                _cli.Recved += OnRecvd;

                _cli.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9527));
            }));
            _timer.Reg(CCall.Create(() =>
            {
                for (var i = 0; i < 8; ++i)
                    Exec();
            }), 10);
            _tick.Ticked += (sender, e) =>
            {
                _task.Exec(CCall.Create(_timer.Exec, e));
            };

            _task.Init();
            _timer.Init();
            _tick.Init();
        }

        public void Destroy()
        {
            _tick.Destroy();
            _timer.Destroy();

            _task.Exec(CCall.Create(() =>
            {
                _cli.Close();
            }));
            _task.Destroy();
        }

        void Exec()
        {
            _cli.Exec();
        }

        void OnConnected(object sender, CDataEventArgs<CTcpClientConnectArgs> e)
        {
            CTcpClient cli = (CTcpClient)sender;

            _log.InfoFormat("OnConnect {0}:{1}:{2}", e.Arg1.Result, e.Arg1.Error, e.Arg1.SocketError);
        }

        void OnClosed(object sender, CDataEventArgs<CTcpClientCloseArgs> e)
        {
            CTcpClient cli = (CTcpClient)sender;

            _log.InfoFormat("OnClosed {0}:{1}:{2}", e.Arg1.Reason, e.Arg1.Error, e.Arg1.SocketError);
        }

        void OnRecvd(object sender, byte[] arg1, int arg2, int arg3)
        {
            CTcpClient cli = (CTcpClient)sender;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var cli = new CCli();
            cli.Init();
            Console.ReadKey();
            cli.Destroy();
        }
    }
}
