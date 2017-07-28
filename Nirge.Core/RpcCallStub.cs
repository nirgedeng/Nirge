/*------------------------------------------------------------------
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
    public class RpcCallStub
    {
        int _serial;
        int _service;
        int _call;
        TaskCompletionSource<ArraySegment<byte>> _awaiter;

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

        public TaskCompletionSource<ArraySegment<byte>> Awaiter
        {
            get
            {
                return _awaiter;
            }
        }

        public RpcCallStub(int serial, int service, int call)
        {
            _serial = serial;
            _service = service;
            _call = call;
            _awaiter = new TaskCompletionSource<ArraySegment<byte>>();
            ;
        }
    }

    public class RpcCallStubProvider
    {
        ILog _log;
        Dictionary<int, RpcCallStub> _stubs;
        int _serial;

        public RpcCallStubProvider(ILog log)
        {
            _log = log;
            _stubs = new Dictionary<int, RpcCallStub>(32);
        }

        public RpcCallStub CreateStub(int service, int call)
        {
            var serial = ++_serial;
            var stub = new RpcCallStub(serial, service, call);
            _stubs.Add(serial, stub);
            return stub;
        }

        public RpcCallStub GetStub(int serial)
        {
            RpcCallStub stub;
            if (_stubs.TryGetValue(serial, out stub))
                return stub;
            return null;
        }

        public bool DelStub(int serial)
        {
            return _stubs.Remove(serial);
        }
    }
}
