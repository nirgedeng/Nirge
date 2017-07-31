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

        public CRpcCallStub(int serial, int service, int call)
        {
            _serial = serial;
            _service = service;
            _call = call;
            _time = DateTime.Now;
            _awaiter = new TaskCompletionSource<Google.Protobuf.WellKnownTypes.Any>();
        }
    }

    public class CRpcCallStubProvider
    {
        ILog _log;
        Dictionary<int, CRpcCallStub> _stubs;
        int _serial;

        public CRpcCallStubProvider(ILog log)
        {
            _log = log;
            _stubs = new Dictionary<int, CRpcCallStub>(32);
        }

        public CRpcCallStub CreateStub(int service, int call)
        {
            var serial = ++_serial;
            var stub = new CRpcCallStub(serial, service, call);
            _stubs.Add(serial, stub);
            return stub;
        }

        public CRpcCallStub GetStub(int serial)
        {
            CRpcCallStub stub;
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
