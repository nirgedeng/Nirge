/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using System.Threading.Tasks;
using log4net;

namespace Nirge.Core
{
    public class CRpcCallerArgs
    {
        TimeSpan _timeout;

        public TimeSpan Timeout
        {
            get
            {
                return _timeout;
            }
        }

        public CRpcCallerArgs(TimeSpan timeout)
        {
            if (timeout == null)
                throw new ArgumentNullException(nameof(timeout));

            _timeout = timeout;
        }
    }

    public abstract class CRpcCaller
    {
        protected static readonly RpcCallArgsEmpty ArgsEmpty = new RpcCallArgsEmpty();

        CRpcCallerArgs _args;
        ILog _log;
        IRpcStream _stream;
        IRpcTransfer _transfer;
        IRpcCallStubProvider _stubs;
        ServiceDescriptor _descriptor;
        protected int _service;
        ulong _callsCount;

        public CRpcCallerArgs Args
        {
            get
            {
                return _args;
            }
        }

        public int Service
        {
            get
            {
                return _service;
            }
        }

        public ulong CallsCount
        {
            get
            {
                return _callsCount;
            }
        }

        public CRpcCaller(CRpcCallerArgs args, ILog log, IRpcStream stream, IRpcTransfer transfer, IRpcCallStubProvider stubs, ServiceDescriptor descriptor, int service)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (log == null)
                throw new ArgumentNullException(nameof(log));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (transfer == null)
                throw new ArgumentNullException(nameof(transfer));
            if (stubs == null)
                throw new ArgumentNullException(nameof(stubs));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            if (service == 0)
                throw new ArgumentOutOfRangeException(nameof(service));

            _args = args;
            _log = log;
            _stream = stream;
            _transfer = transfer;
            _stubs = stubs;
            _descriptor = descriptor;
            _service = service;
            _callsCount = 0;
        }

        void Call<TArgs>(int channel, TArgs args, RpcCallReq pkg) where TArgs : IMessage<TArgs>
        {
            pkg.Args = args.ToByteString();
            _transfer.Send(channel, pkg);
            ++_callsCount;
        }

        protected void Call<TArgs>(int channel, int call, TArgs args) where TArgs : IMessage<TArgs>
        {
            if (channel < 0)
                throw new ArgumentOutOfRangeException(nameof(channel));
            if (call < 0)
                throw new ArgumentOutOfRangeException(nameof(call));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var pkg = new RpcCallReq()
            {
                Service = _service,
                Call = call,
            };

            Call<TArgs>(channel, args, pkg);
        }

        protected async Task<TRet> CallAsync<TArgs, TRet>(int channel, int call, TArgs args) where TArgs : IMessage<TArgs> where TRet : IMessage<TRet>, new()
        {
            if (channel < 0)
                throw new ArgumentOutOfRangeException(nameof(channel));
            if (call < 0)
                throw new ArgumentOutOfRangeException(nameof(call));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var stub = _stubs.CreateStub(_descriptor, _service, call, _args.Timeout);

            var pkg = new RpcCallReq()
            {
                Service = _service,
                Call = call,
                Serial = stub.Serial,
            };

            try
            {
                Call<TArgs>(channel, args, pkg);
            }
            catch (Exception ex)
            {
                _stubs.DelStub(stub);
                throw ex;
            }

            var task = stub.Wait.Task;
            await task;
            var ret = new TRet();
            ret.MergeFrom(task.Result);

            return ret;
        }
    }
}
