﻿/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Reflection;
using log4net.Config;
using System.Linq;
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
            IConfigurationRoot _cfg;
            ILog _log;
            ITcpClientCache _cache;
            CRpcStream _stream;
            CRpcCallStubProvider _stubs;
            gargs _gargs;
            pargs _pargs;
            qargs _qargs;
            CTcpClient _cli;
            CRpcCommunicator _communicator;
            CGameRpcCaller _caller;

            public CTcpClient Cli
            {
                get => _cli;
            }

            public CClient(IConfigurationRoot cfg, ILog log, ITcpClientCache cache, CRpcStream stream, CRpcCallStubProvider stubs, gargs gargs, pargs pargs, qargs qargs)
            {
                _cfg = cfg;
                _log = log;
                _cache = cache;
                _stream = stream;
                _stubs = stubs;
                _gargs = gargs;
                _pargs = pargs;
                _qargs = qargs;
                _cli = new CTcpClient(new CTcpClientArgs(), _log, _cache);
                _communicator = new CClientRpcCommunicator(_log, _cli);
                _caller = new CGameRpcCaller(new CRpcCallerArgs(TimeSpan.FromMinutes(8f), false), log, stream, _communicator, stubs);
            }

            public void Init()
            {
                _cli.Connected += OnConnected;
                _cli.Closed += OnClosed;
                _cli.Recved += OnRecvd;
                _cli.Connect(new IPEndPoint(IPAddress.Parse(_cfg.GetValue<string>("Addr", "127.0.0.1")), 9527));
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
                _caller.g(_gargs);
            }

            async void g()
            {
                try
                {
                    await _caller.h();
                }
                catch
                {
                }
                try
                {
                    await _caller.p(_pargs);
                }
                catch
                {
                }
                try
                {
                    await _caller.q(_qargs);
                }
                catch
                {
                }
            }

            void OnConnected(object sender, CDataEventArgs<CTcpClientConnectArgs> e)
            {
                CTcpClient cli = (CTcpClient)sender;

                _log.InfoFormat("OnConnect {0}:{1}:{2}", e.Arg1.Result, e.Arg1.Error, e.Arg1.SocketError);

                f();
                g();
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
                        var rsp = RpcCallRsp.Parser.ParseFrom(_stream.Input.Buf);
                        _stubs.Exec(rsp);
                    }
                    break;
                case eRpcProto.RpcCallExceptionRsp:
                    {
                        _stream.Input.Buf.SetBuf(arg1, arg2 + 4, arg3 - 4);
                        var rsp = RpcCallExceptionRsp.Parser.ParseFrom(_stream.Input.Buf);
                        _stubs.Exec(rsp);
                    }
                    break;
                }
            }
        }

        IConfigurationRoot _cfg;
        ILog _log;
        CTasker _task;
        CTaskTimer _timer;
        CTicker _tick;
        int _call;

        ITcpClientCache _cache;
        List<CClient> _clis;

        CRpcStream _stream;
        CRpcCallStubProvider _stubs;
        gargs _gargs;
        pargs _pargs;
        qargs _qargs;

        public void Init(IConfigurationRoot cfg)
        {
            _cfg = cfg;
            _log = LogManager.Exists("cli", "all");

            _task = new CTasker(_log);
            _timer = new CTaskTimer(_task, _log);
            _tick = new CTicker();

            _cache = new CTcpClientCacheEmpty(new CTcpClientCacheArgs(25600, 12800, 6400, 25600, 12800, 6400));
            _clis = new List<CClient>();

            _stream = new CRpcStream(new CRpcInputStream(), new CRpcOutputStream(new byte[1024], 0, 1024));
            _stubs = new CRpcCallStubProvider(new CRpcCallStubArgs(false, false), _log);

            _gargs = new gargs() { A = 1, B = 2, C = 3, };
            _gargs.D.AddRange(new int[] { 1, 2, 3, 4, });
            _pargs = new pargs() { A = 4, B = 5, C = 6, };
            _pargs.D.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, });
            _qargs = new qargs() { A = 7, B = 8, C = 9, };
            _qargs.D.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });

            for (int i = 0, len = _cfg.GetValue<int>("Clis", 1024); i < len; ++i)
                _clis.Add(new CClient(_cfg, _log, _cache, _stream, _stubs, _gargs, _pargs, _qargs));

            _task.Exec(CCall.Create(() =>
            {
                foreach (var i in _clis)
                    i.Init();
            }));
            _timer.Reg(CCall.Create(() =>
            {
                for (var i = 0; i < 1; ++i)
                    Exec();
            }), 40);
            _call = _timer.Reg(CCall.Create(() =>
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
            _tick.Init();
            _timer.Init();
        }

        public void Destroy()
        {
            _task.Exec(CCall.Create(() =>
            {
                _stubs.Destroy();
                _timer.Unreg(_call);
                _stream.Dispose();
            }));

            _task.Exec(CCall.Create(() =>
            {
                foreach (var i in _clis)
                    i.Destroy();
            }));
            for (; ; )
            {
                if (_clis.All(e => e.Cli.State == eTcpClientState.Closed))
                    break;
            }

            _task.Exec(CCall.Create(() =>
            {
                _timer.Destroy();
            }));
            _tick.Destroy();
            _task.Destroy();
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
            cli.Init(new ConfigurationBuilder().AddXmlFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "../../Net.multi.cfg.cli.xml")).Build());
            Console.ReadKey();
            cli.Destroy();
        }
    }
}
