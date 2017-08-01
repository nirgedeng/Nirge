﻿/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using System;

namespace Nirge.Core
{
    public class CRpcCallStub
    {
        int _serial;
        int _service;
        int _call;
        DateTime _time;
        TaskCompletionSource<Google.Protobuf.WellKnownTypes.Any> _awaiter;

        public int Serial
        {
            get
            {
                return _serial;
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

        public TaskCompletionSource<Google.Protobuf.WellKnownTypes.Any> Awaiter
        {
            get
            {
                return _awaiter;
            }
        }

        public CRpcCallStub(int serial, int service, int call, DateTime time)
        {
            _serial = serial;
            _service = service;
            _call = call;
            _time = time;
            _awaiter = new TaskCompletionSource<Google.Protobuf.WellKnownTypes.Any>();
        }
    }

    public class CRpcCallStubProvider
    {
        ILog _log;
        List<CRpcCallStub> _stubs;
        Dictionary<int, CRpcCallStub> _stubsDict;
        int _serial;

        public CRpcCallStubProvider(ILog log)
        {
            _log = log;
            _stubs = new List<CRpcCallStub>(32);
            _stubsDict = new Dictionary<int, CRpcCallStub>(32);
        }

        public void Init()
        {
        }

        public void Destroy()
        {
            _stubs.Clear();
            _stubsDict.Clear();
        }

        public int CreateSerial()
        {
            return ++_serial;
        }

        public CRpcCallStub CreateStub(int serial, int service, int call, TimeSpan timeout)
        {
            var stub = new CRpcCallStub(serial, service, call, DateTime.Now.Add(timeout));
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
            for (var i = _stubs.Count - 1; i >= 0; --i)
            {
                CRpcCallStub stub = _stubs[i];

                if (DateTime.Now < stub.Time)
                    continue;

                DelStub(stub);

                stub.Awaiter.SetException(new CCallerTimeoutRpcException());
            }
        }

        public void Exec(RpcCallRsp rsp)
        {
            var stub = GetStub(rsp.Serial);
            if (stub == null)
            {
                _log.InfoFormat("[Rpc]RpcCallStub.Exec !stub, serial:\"{0}\", service:\"{1}\", call:\"{2}\", ret:\"{3}\"", rsp.Serial, rsp.Service, rsp.Call, rsp.Ret);
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
                _log.InfoFormat("[Rpc]RpcCallStub.Exec !stub, serial:\"{0}\", service:\"{1}\", call:\"{2}\", exception:\"{3}\"", rsp.Serial, rsp.Service, rsp.Call, rsp.Exception);
                return;
            }

            DelStub(stub);

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
