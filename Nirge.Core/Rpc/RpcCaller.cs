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
    public class CRpcCaller
    {
        ILog _log;
        CRpcCallStubProvider _stubs;
        CRpcChannel _channel;

        public CRpcCaller(ILog log, CRpcCallStubProvider stubs, CRpcChannel channel)
        {
            _log = log;
            _stubs = stubs;
            _channel = channel;
        }

        async Task<ArraySegment<byte>> Call(int service, int call, ArraySegment<byte> args)
        {
            var stub = _stubs.CreateStub(service, call);
            var task = stub.Awaiter.Task;

            var pkg = new C2sRpcCall()
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
