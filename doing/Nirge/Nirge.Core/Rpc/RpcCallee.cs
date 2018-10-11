/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using log4net;

namespace Nirge.Core
{
    public class CRpcCalleeArgs
    {
        public CRpcCalleeArgs()
        {
        }
    }

    public abstract class CRpcCallee<TRpcService> where TRpcService : IRpcService
    {
        protected static readonly RpcCallArgsEmpty ArgsEmpty = new RpcCallArgsEmpty();

        CRpcCalleeArgs _args;
        ILog _log;
        IRpcStream _stream;
        IRpcTransfer _transfer;
        ServiceDescriptor _descriptor;
        protected TRpcService _service;

        public CRpcCallee(CRpcCalleeArgs args, ILog log, IRpcStream stream, IRpcTransfer transfer, ServiceDescriptor descriptor, TRpcService service)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (log == null)
                throw new ArgumentNullException(nameof(log));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (transfer == null)
                throw new ArgumentNullException(nameof(transfer));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _args = args;
            _log = log;
            _stream = stream;
            _transfer = transfer;
            _descriptor = descriptor;
            _service = service;
        }

        eRpcException Call<TArgs, TRet>(int channel, RpcCallReq req, Func<int, TArgs, TRet> call, out TArgs args, out TRet ret) where TArgs : IMessage<TArgs>, new() where TRet : IMessage<TRet>
        {
            args = new TArgs();
            ret = default(TRet);

            try
            {
                args.MergeFrom(req.Args);
            }
            catch (Exception exception)
            {
                _log.WriteLine(eLogPattern.Error, $"RPC callee Call exception " +
                    $"channel {channel} serial {req.Serial} service {req.Service} {_descriptor.FullName} call {req.Call} {_descriptor.GetCall(req.Call)?.Name} exception {eRpcException.CalleeArgsDeserialize}", exception);
                return eRpcException.CalleeArgsDeserialize;
            }

            try
            {
                ret = call(channel, args);
            }
            catch (Exception exception)
            {
                _log.WriteLine(eLogPattern.Error, $"RPC callee Call exception " +
                    $"channel {channel} serial {req.Serial} service {req.Service} {_descriptor.FullName} call {req.Call} {_descriptor.GetCall(req.Call)?.Name} args {args} exception {eRpcException.CalleeExec}", exception);
                return eRpcException.CalleeExec;
            }

            return eRpcException.None;
        }

        protected void Call<TArgs, TRet>(int channel, RpcCallReq req, Func<int, TArgs, TRet> call) where TArgs : IMessage<TArgs>, new() where TRet : IMessage<TRet>
        {
            if (channel < 0)
                throw new ArgumentOutOfRangeException(nameof(channel));
            if (req == null)
                throw new ArgumentNullException(nameof(req));
            if (call == null)
                throw new ArgumentNullException(nameof(call));

            Call<TArgs, TRet>(channel, req, call, out var args, out var ret);
        }

        eRpcException CallAsync<TArgs, TRet>(int channel, RpcCallReq req, Func<int, TArgs, TRet> call, out TArgs args, out TRet ret) where TArgs : IMessage<TArgs>, new() where TRet : IMessage<TRet>
        {
            var e = Call<TArgs, TRet>(channel, req, call, out args, out ret);
            switch (e)
            {
            case eRpcException.None:
                var pkg = new RpcCallRsp()
                {
                    Serial = req.Serial,
                    Service = req.Service,
                    Call = req.Call,
                };

                try
                {
                    pkg.Ret = ret.ToByteString();
                }
                catch (Exception exception)
                {
                    _log.WriteLine(eLogPattern.Error, $"RPC callee Call exception " +
                        $"channel {channel} serial {req.Serial} service {req.Service} {_descriptor.FullName} call {req.Call} {_descriptor.GetCall(req.Call)?.Name} args {args} ret {ret} exception {eRpcException.CalleeRetSerialize}", exception);
                    return eRpcException.CalleeRetSerialize;
                }

                try
                {
                    _transfer.Send(channel, pkg);
                }
                catch (Exception exception)
                {
                    _log.WriteLine(eLogPattern.Error, $"RPC callee Call exception " +
                        $"channel {channel} serial {req.Serial} service {req.Service} {_descriptor.FullName} call {req.Call} {_descriptor.GetCall(req.Call)?.Name} args {args} ret {ret} exception {eRpcException.CalleeTransfer}", exception);
                    return eRpcException.CalleeTransfer;
                }

                return eRpcException.None;
            default:
                return e;
            }
        }

        protected void CallAsync<TArgs, TRet>(int channel, RpcCallReq req, Func<int, TArgs, TRet> call) where TArgs : IMessage<TArgs>, new() where TRet : IMessage<TRet>
        {
            if (channel < 0)
                throw new ArgumentOutOfRangeException(nameof(channel));
            if (req == null)
                throw new ArgumentNullException(nameof(req));
            if (call == null)
                throw new ArgumentNullException(nameof(call));

            var e = CallAsync<TArgs, TRet>(channel, req, call, out var args, out var ret);
            switch (e)
            {
            case eRpcException.None:
            case eRpcException.CalleeTransfer:
                break;
            default:
                var pkg = new RpcCallExceptionRsp()
                {
                    Serial = req.Serial,
                    Service = req.Service,
                    Call = req.Call,
                    Exception = (int)e,
                };

                try
                {
                    _transfer.Send(channel, pkg);
                }
                catch (Exception exception)
                {
                    _log.WriteLine(eLogPattern.Error, $"RPC callee Call exception " +
                        $"channel {channel} serial {req.Serial} service {req.Service} {_descriptor.FullName} call {req.Call} {_descriptor.GetCall(req.Call)?.Name} args {args} ret {ret} exception {e}", exception);
                }
                break;
            }
        }

        public virtual void Call(int channel, RpcCallReq req)
        {
            if (channel < 0)
                throw new ArgumentOutOfRangeException(nameof(channel));
            if (req == null)
                throw new ArgumentNullException(nameof(req));

            _log.WriteLine(eLogPattern.Warn, $"RPC callee Call base " +
                $"channel {channel} serial {req.Serial} service {req.Service} {_descriptor.FullName} call {req.Call} {_descriptor.GetCall(req.Call)?.Name} req {req}");
        }
    }
}
