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
    public class RpcCaller
    {
        ILog _log;
        RpcCallStubProvider _stubs;
        RpcChannel _channel;

        public RpcCaller(ILog log, RpcCallStubProvider stubs, RpcChannel channel)
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
            catch (RpcException exception)
            {
            }
            catch (Exception exception)
            {
            }

            return task.Result;
        }
    }
}
