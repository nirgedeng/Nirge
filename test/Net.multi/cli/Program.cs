/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;
using log4net.Config;
using System.Linq;
using System.Text;
using Nirge.Core;
using System.Net;
using System.IO;
using log4net;
using System;

namespace cli
{
    class CCli
    {
        class CClient
        {
            ILog _log;
            TcpClientCache _cache;
            CRpcStream _stream;
            CRpcCallStubProvider _stubs;
            CRpcCommunicator _communicator;
            CGameRpcCaller _caller;
            CTcpClient _cli;

            public CClient(ILog log, TcpClientCache cache, CRpcStream stream, CRpcCallStubProvider stubs)
            {
                _log = log;
                _cache = cache;
                _stream = stream;
                _stubs = stubs;
                _cli = new CTcpClient(new CTcpClientArgs(), log, cache);
                _communicator = new CClientRpcCommunicator(log, _cli);
                _caller = new CGameRpcCaller(new CRpcCallerArgs(TimeSpan.FromMinutes(8f), false), log, stream, _communicator, stubs);
            }

            public void Init()
            {
                _cli.Connected += OnConnected;
                _cli.Closed += OnClosed;
                _cli.Recved += OnRecvd;
                _cli.Connect(new IPEndPoint(IPAddress.Parse("10.12.236.197"), 9527));
            }

            public void Destroy()
            {
                _cli.Close();
            }

            public void Exec()
            {
                _cli.Exec();
            }

            public void Call()
            {
                f();
                g();
            }

            void f()
            {
                _caller.f();
                var gargs = new gargs() { A = 1, B = 2, C = 3, };
                gargs.D.AddRange(new int[] { 1, 2, 3, 4, });
                _caller.g(gargs);
            }

            async void g()
            {
                await _caller.h();
                var pargs = new pargs() { A = 4, B = 5, C = 6, };
                pargs.D.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, });
                await _caller.p(pargs);
                var qargs = new qargs() { A = 7, B = 8, C = 9, };
                qargs.D.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
                await _caller.q(qargs);
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
        }

        ILog _log;
        CTasker _task;
        CTaskTimer _timer;
        CTicker _tick;

        TcpClientCache _cache;
        List<CClient> _clis;

        CRpcStream _stream;
        CRpcCallStubProvider _stubs;

        public void Init()
        {
            _log = LogManager.Exists("cli", "all");

            _task = new CTasker(_log);
            _timer = new CTaskTimer(_task, _log);
            _tick = new CTicker();

            _cache = new TcpClientCache(new TcpClientCacheArgs(25600, 12800, 6400, 25600, 12800, 6400));
            _clis = new List<CClient>();

            _stream = new CRpcStream(new CRpcInputStream(), new CRpcOutputStream(new byte[1024], 0, 1024));
            _stubs = new CRpcCallStubProvider(new CRpcCallStubArgs(false, false), _log);

            for (int i = 0; i < 1024; ++i)
                _clis.Add(new CClient(_log, _cache, _stream, _stubs));

            _task.Exec(CCall.Create(() =>
            {
                foreach (var i in _clis)
                    i.Init();
            }));
            _timer.Reg(CCall.Create(() =>
            {
                for (var i = 0; i < 2; ++i)
                    Exec();
            }), 10);
            _timer.Reg(CCall.Create(() =>
            {
                foreach (var i in _clis)
                    i.Call();
            }), 200/*, 128*/);
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
                foreach (var i in _clis)
                    i.Destroy();
            }));

            _task.Destroy();
            _timer.Destroy();
            _tick.Destroy();
        }

        void Exec()
        {
            foreach (var i in _clis)
                i.Exec();
            _stubs.Exec();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure(LogManager.CreateRepository("cli"), new FileInfo("../../Net.multi.log.cli.xml"));

            var cli = new CCli();
            cli.Init();
            Console.ReadKey();
            cli.Destroy();
        }
    }
}
