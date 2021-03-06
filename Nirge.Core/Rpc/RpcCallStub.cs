﻿/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using Google.Protobuf.Reflection;
using System.Threading.Tasks;
using System.Threading;
using Google.Protobuf;
using log4net;
using System;

namespace Nirge.Core
{
    public class CRpcCallStub
    {
        int _serial;
        ServiceDescriptor _descriptor;
        int _service;
        int _call;
        DateTime _time;
        TaskCompletionSource<ByteString> _awaiter;

        public int Serial
        {
            get
            {
                return _serial;
            }
        }

        public ServiceDescriptor Descriptor
        {
            get
            {
                return _descriptor;
            }
        }

        public int Service
        {
            get
            {
                return _service;
            }
        }

        public int Call
        {
            get
            {
                return _call;
            }
        }

        public DateTime Time
        {
            get
            {
                return _time;
            }
        }

        public TaskCompletionSource<ByteString> Awaiter
        {
            get
            {
                return _awaiter;
            }
        }

        public CRpcCallStub(int serial, ServiceDescriptor descriptor, int service, int call, DateTime time)
        {
            _serial = serial;
            _descriptor = descriptor;
            _service = service;
            _call = call;
            _time = time;
            _awaiter = new TaskCompletionSource<ByteString>();
        }
    }

    public class CRpcCallStubArgs
    {
        bool _logCall;
        bool _logTimeout;

        public bool LogCall
        {
            get
            {
                return _logCall;
            }
        }

        public bool LogTimeout
        {
            get
            {
                return _logTimeout;
            }
        }

        public CRpcCallStubArgs(bool logCall, bool logTimeout)
        {
            _logCall = logCall;
            _logTimeout = logTimeout;
        }
    }

    public class CRpcCallStubProvider
    {
        CRpcCallStubArgs _args;
        ILog _log;
        List<CRpcCallStub> _stubs;
        Dictionary<int, CRpcCallStub> _stubsDict;
        int _serial;

        public CRpcCallStubProvider(CRpcCallStubArgs args, ILog log)
        {
            _args = args;
            _log = log;
            _stubs = new List<CRpcCallStub>(32);
            _stubsDict = new Dictionary<int, CRpcCallStub>(32);
        }

        public void Init()
        {
        }

        public void Destroy()
        {
            Break();
        }

        public int CreateSerial()
        {
            return ++_serial;
        }

        public CRpcCallStub CreateStub(int serial, ServiceDescriptor descriptor, int service, int call, TimeSpan timeout)
        {
            var stub = new CRpcCallStub(serial, descriptor, service, call, DateTime.Now.Add(timeout));
            _stubs.Add(stub);
            _stubsDict.Add(serial, stub);
            return stub;
        }

        CRpcCallStub GetStub(int serial)
        {
            CRpcCallStub stub;
            if (_stubsDict.TryGetValue(serial, out stub))
                return stub;
            return null;
        }

        void DelStub(CRpcCallStub stub)
        {
            _stubs.Remove(stub);
            _stubsDict.Remove(stub.Serial);
        }

        public void Exec()
        {
            if (_stubs.Count == 0)
                return;

            for (var i = _stubs.Count - 1; i >= 0; --i)
            {
                CRpcCallStub stub = _stubs[i];

                if (DateTime.Now < stub.Time)
                    continue;

                DelStub(stub);

                if (_args.LogTimeout)
                {
                    _log.InfoFormat("[Rpc]RpcCallStub.Exec timeout, serial:\"{0}\", service:\"{1},{2}\", call:\"{3},{4}\", time:\"{5}\""
                        , stub.Serial
                        , stub.Service
                        , stub.Descriptor.FullName
                        , stub.Call
                        , stub.Descriptor.GetRpcServiceCall(stub.Call).Name
                        , stub.Time);
                }

                stub.Awaiter.SetException(new CCallerTimeoutRpcException());
            }
        }

        public void Break()
        {
            if (_stubs.Count == 0)
                return;

            for (var i = _stubs.Count - 1; i >= 0; --i)
            {
                CRpcCallStub stub = _stubs[i];

                DelStub(stub);

                try
                {
                    stub.Awaiter.SetException(new CCallerBreakRpcException());
                }
                catch (Exception exception)
                {
                    _log.Error(string.Format("[Rpc]RpcCallStub.Break exception, serial:\"{1}\", service:\"{2},{3}\", call:\"{4},{5}\", time:\"{6}\""
                        , stub.Serial
                        , stub.Service
                        , stub.Descriptor.FullName
                        , stub.Call
                        , stub.Descriptor.GetRpcServiceCall(stub.Call).Name
                        , stub.Time), exception);
                }
            }
        }

        public void Exec(RpcCallRsp rsp)
        {
            var stub = GetStub(rsp.Serial);
            if (stub == null)
            {
                _log.InfoFormat("[Rpc]RpcCallStub.Exec !stub, serial:\"{0}\", service:\"{1}\", call:\"{2}\", ret:\"{3}\""
                    , rsp.Serial
                    , rsp.Service
                    , rsp.Call
                    , rsp.Ret);
                return;
            }

            DelStub(stub);

            stub.Awaiter.SetResult(rsp.Ret);
        }

        public void Exec(RpcCallExceptionRsp rsp)
        {
            var stub = GetStub(rsp.Serial);
            if (stub == null)
            {
                _log.InfoFormat("[Rpc]RpcCallStub.Exec !stub, serial:\"{0}\", service:\"{1}\", call:\"{2}\", exception:\"{3}\""
                    , rsp.Serial
                    , rsp.Service
                    , rsp.Call
                    , rsp.Exception);
                return;
            }

            DelStub(stub);

            if (_args.LogCall)
            {
                _log.InfoFormat("[Rpc]RpcCallStub.Exec ExceptionRsp, serial:\"{0}\", service:\"{1},{2}\", call:\"{3},{4}\", time:\"{5}\", exception:\"{6}\""
                    , stub.Serial
                    , stub.Service
                    , stub.Descriptor.FullName
                    , stub.Call
                    , stub.Descriptor.GetRpcServiceCall(stub.Call).Name
                    , stub.Time
                    , rsp.Exception);
            }

            switch ((eRpcException)rsp.Exception)
            {
            case eRpcException.CalleeArgsDeserialize:
                stub.Awaiter.SetException(new CCalleeArgsDeserializeRpcException());
                break;
            case eRpcException.CalleeExec:
                stub.Awaiter.SetException(new CCalleeExecRpcException());
                break;
            case eRpcException.CalleeRetSerialize:
                stub.Awaiter.SetException(new CCalleeRetSerializeRpcException());
                break;
            default:
                stub.Awaiter.SetException(new CRpcException());
                break;
            }
        }
    }
}
