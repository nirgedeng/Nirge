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
    public class CRpcCallerArgs
    {
        int _timeout;

        public int Timeout
        {
            get
            {
                return _timeout;
            }
        }

        public CRpcCallerArgs(int timeout)
        {
            _timeout = timeout;
        }
    }

    public class CRpcCaller
    {
        CRpcCallerArgs _args;
        ILog _log;
        CRpcStream _stream;
        CRpcCommunicator _communicator;
        CRpcCallStubProvider _stubs;

        public CRpcCaller(CRpcCallerArgs args, ILog log, CRpcStream stream, CRpcCommunicator communicator, CRpcCallStubProvider stubs)
        {
            _args = args;
            _log = log;
            _stream = stream;
            _communicator = communicator;
            _stubs = stubs;
        }

        async Task<Google.Protobuf.WellKnownTypes.Any> Call(int service, int call, Google.Protobuf.WellKnownTypes.Any args)
        {
            var stub = _stubs.CreateStub(service, call);
            var task = stub.Awaiter.Task;

            var pkg = new RpcCallReq()
            {
                Serial = stub.Serial,
                Service = stub.Service,
                Call = stub.Call,
            };

            try
            {
                await task;
            }
            catch
            {
            }

            return task.Result;
        }
    }
}
