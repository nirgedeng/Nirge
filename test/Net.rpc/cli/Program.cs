﻿/*------------------------------------------------------------------
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

        CRpcCommunicator _communicator;
        CRpcStream _stream;
        CRpcCallStubProvider _stubs;
        CGameRpcCaller _caller;

        public void Init()
        {
            _log = LogManager.Exists("all");

            _task = new CTasker(_log);
            _timer = new CTaskTimer(_task, _log);
            _tick = new CTicker();

            _cache = new TcpClientCache(new TcpClientCacheArgs(25600, 12800, 6400, 25600, 12800, 6400));
            _cli = new CTcpClient(new CTcpClientArgs(), _log, _cache);

            _communicator = new CClientRpcCommunicator(_log, _cli);
            _stream = new CRpcStream(new CRpcInputStream(), new CRpcOutputStream(new byte[1024], 0, 1024));
            _stubs = new CRpcCallStubProvider(new CRpcCallStubArgs(true, true), _log);
            _caller = new CGameRpcCaller(new CRpcCallerArgs(TimeSpan.FromSeconds(8f), true), _log, _stream, _communicator, _stubs);

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

            _stubs.Init();
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
                _stubs.Destroy();
                _stream.Dispose();
            }));

            _task.Exec(CCall.Create(() =>
            {
                _cli.Close();
            }));
            _task.Destroy();
        }

        void Exec()
        {
            _cli.Exec();
            _stubs.Exec();
        }

        void OnConnected(object sender, CDataEventArgs<CTcpClientConnectArgs> e)
        {
            CTcpClient cli = (CTcpClient)sender;

            _log.InfoFormat("OnConnect {0}:{1}:{2}", e.Arg1.Result, e.Arg1.Error, e.Arg1.SocketError);

            _caller.f();
            _caller.g(new gargs() { A = 1, B = 2, C = 3, });
            //f();
        }

        void OnClosed(object sender, CDataEventArgs<CTcpClientCloseArgs> e)
        {
            CTcpClient cli = (CTcpClient)sender;

            _log.InfoFormat("OnClosed {0}:{1}:{2}", e.Arg1.Reason, e.Arg1.Error, e.Arg1.SocketError);
        }

        void OnRecvd(object sender, byte[] arg1, int arg2, int arg3)
        {
            CTcpClient cli = (CTcpClient)sender;

            var cmd = BitConverter.ToInt32(arg1, arg2);

            switch ((eRpcProto)cmd)
            {
            case eRpcProto.RpcCallRsp:
                {
                    _stream.Input.Buf.SetBuf(arg1, arg2 + 4, arg3 - 4);
                    var rsp = RpcCallRsp.Parser.ParseFrom(_stream.Input.Stream);
                    _stubs.Exec(rsp);
                }
                break;
            case eRpcProto.RpcCallExceptionRsp:
                {
                    _stream.Input.Buf.SetBuf(arg1, arg2 + 4, arg3 - 4);
                    var rsp = RpcCallExceptionRsp.Parser.ParseFrom(_stream.Input.Stream);
                    _stubs.Exec(rsp);
                }
                break;
            }
        }

        async void f()
        {
            await _caller.h();
            await _caller.p(new pargs() { A = 1, B = 2, C = 3, });
            var qret = _caller.q(new qargs() { A = 1, B = 2, C = 3, });
            await qret;
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