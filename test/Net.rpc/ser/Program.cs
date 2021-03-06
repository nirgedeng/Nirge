﻿/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;
using Google.Protobuf;
using log4net.Config;
using System.Linq;
using System.Text;
using Nirge.Core;
using System.Net;
using System.IO;
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

        CRpcCommunicator _communicator;
        CRpcStream _stream;
        CGameRpcService _service;
        CGameRpcCallee _callee;
        CRpcCallStubProvider _stubs;
        CGameRpcCaller _caller;

        public void Init()
        {
            _log = LogManager.Exists("ser", "all");

            _task = new CTasker(_log);
            _timer = new CTaskTimer(_task, _log);
            _tick = new CTicker();

            _ser = new CTcpServer(_log);
            _ser.Closed += Ser_Closed;
            _ser.CliConnected += Ser_CliConnected;
            _ser.CliClosed += Ser_CliClosed;
            _ser.CliRecved += Ser_CliRecved;

            _communicator = new CServerRpcCommunicator(_log, _ser);
            _stream = new CRpcStream(new CRpcInputStream(), new CRpcOutputStream(new byte[1024], 0, 1024));
            _service = new CGameRpcService();
            _callee = new CGameRpcCallee(new CRpcCalleeArgs(false), _log, _stream, _communicator, _service);
            _stubs = new CRpcCallStubProvider(new CRpcCallStubArgs(false, false), _log);
            _caller = new CGameRpcCaller(new CRpcCallerArgs(TimeSpan.FromMinutes(8f), false), _log, _stream, _communicator, _stubs);

            _task.Exec(CCall.Create(() =>
            {
                _ser.Open(new IPEndPoint(IPAddress.Any, 9527));
            }));
            _timer.Reg(CCall.Create(() =>
            {
                for (int i = 0; i < 8; ++i)
                    Exec();
            }), 40);
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
            _task.Exec(CCall.Create(() =>
            {
                _stubs.Destroy();
                _stream.Dispose();
            }));

            _task.Exec(CCall.Create(() =>
            {
                _ser.Close();
            }));

            _task.Destroy();
            _timer.Destroy();
            _tick.Destroy();
        }

        void Exec()
        {
            _ser.Exec();
            _stubs.Exec();
        }

        private void Ser_Closed(object sender, CDataEventArgs<CTcpServerCloseArgs> e)
        {
        }

        private void Ser_CliRecved(object sender, int cli, byte[] arg2, int arg3, int arg4)
        {
            var cmd = BitConverter.ToInt32(arg2, arg3);

            switch ((eRpcProto)cmd)
            {
            case eRpcProto.RpcCallReq:
                {
                    _stream.Input.Buf.SetBuf(arg2, arg3 + 4, arg4 - 4);
                    var req = RpcCallReq.Parser.ParseFrom(_stream.Input.Stream);
                    _callee.Call(cli, req);
                }
                break;
            case eRpcProto.RpcCallRsp:
                {
                    _stream.Input.Buf.SetBuf(arg2, arg3 + 4, arg4 - 4);
                    var rsp = RpcCallRsp.Parser.ParseFrom(_stream.Input.Stream);
                    _stubs.Exec(rsp);
                }
                break;
            case eRpcProto.RpcCallExceptionRsp:
                {
                    _stream.Input.Buf.SetBuf(arg2, arg3 + 4, arg4 - 4);
                    var rsp = RpcCallExceptionRsp.Parser.ParseFrom(_stream.Input.Stream);
                    _stubs.Exec(rsp);
                }
                break;
            }
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

            f(cli);
            g(cli);
        }

        void f(int cli)
        {
            _caller.f(cli);
            _caller.g(new gargs() { A = 1, B = 2, C = 3, }, cli);
        }

        async void g(int cli)
        {
            await _caller.h(cli);
            await _caller.p(new pargs() { A = 4, B = 5, C = 6, }, cli);
            var qret = _caller.q(new qargs() { A = 7, B = 8, C = 9, }, cli);
            await qret;
            Console.WriteLine(qret.Result);
        }

        void h(int cli)
        {
            g(cli);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure(LogManager.CreateRepository("ser"), new FileInfo("../../Net.rpc.log.ser.xml"));

            var ser = new CSer();
            ser.Init();
            Console.ReadKey();
            ser.Destroy();
        }
    }
}
