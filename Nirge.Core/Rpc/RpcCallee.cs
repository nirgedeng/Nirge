/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using Google.Protobuf.Reflection;
using System.Reflection;
using System.Threading;
using Google.Protobuf;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using System;

namespace Nirge.Core
{
    #region

    public class CRpcCalleeArgs
    {
        bool _logCall;

        public bool LogCall
        {
            get
            {
                return _logCall;
            }
        }

        public CRpcCalleeArgs(bool logCall)
        {
            _logCall = logCall;
        }
    }

    #endregion

    public abstract class CRpcCallee<TRpcService> where TRpcService : IRpcService
    {
        protected static RpcCallArgsEmpty ArgsEmpty = new RpcCallArgsEmpty();

        CRpcCalleeArgs _args;
        ILog _log;
        CRpcStream _stream;
        CRpcCommunicator _communicator;
        ServiceDescriptor _descriptor;
        protected TRpcService _service;

        public CRpcCallee(CRpcCalleeArgs args, ILog log, CRpcStream stream, CRpcCommunicator communicator, ServiceDescriptor descriptor, TRpcService service)
        {
            _args = args;
            _log = log;
            _stream = stream;
            _communicator = communicator;
            _descriptor = descriptor;
            _service = service;
        }

        eRpcException Call<TArgs, TRet>(int channel, RpcCallReq req, Func<int, TArgs, TRet> call, out TArgs args, out TRet ret) where TArgs : IMessage, new() where TRet : IMessage
        {
            args = new TArgs();
            ret = default(TRet);

            try
            {
                args.MergeFrom(req.Args);
            }
            catch (Exception exception)
            {
                _log.Error(string.Format("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\"", channel, req.Serial, _descriptor.FullName, _descriptor.GetRpcServiceCall(req.Call).Name), exception);
                return eRpcException.CalleeArgsDeserialize;
            }

            try
            {
                ret = call(channel, args);
            }
            catch (Exception exception)
            {
                _log.Error(string.Format("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\"", channel, req.Serial, _descriptor.FullName, _descriptor.GetRpcServiceCall(req.Call).Name, args), exception);
                return eRpcException.CalleeExec;
            }

            if (_args.LogCall)
            {
                _log.InfoFormat("[Rpc]RpcCallee.Call Req, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\"", channel, req.Serial, _descriptor.FullName, _descriptor.GetRpcServiceCall(req.Call).Name, args, ret);
            }

            return eRpcException.None;
        }

        eRpcException CallAsync<TArgs, TRet>(int channel, RpcCallReq req, Func<int, TArgs, TRet> call, out TArgs args, out TRet ret) where TArgs : IMessage, new() where TRet : IMessage
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
                    _log.Error(string.Format("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\"", channel, req.Serial, _descriptor.FullName, _descriptor.GetRpcServiceCall(req.Call).Name, args, ret), exception);
                    return eRpcException.CalleeRetSerialize;
                }

                _stream.Output.Clear();
                var cmd = BitConverter.GetBytes((int)eRpcProto.RpcCallRsp);

                try
                {
                    _stream.Output.Buf.Write(cmd, 0, cmd.Length);
                }
                catch (Exception exception)
                {
                    _stream.Output.Reset();
                    _log.Error(string.Format("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\"", channel, req.Serial, _descriptor.FullName, _descriptor.GetRpcServiceCall(req.Call).Name, args, ret), exception);
                    return eRpcException.CalleeRetSerialize;
                }

                try
                {
                    pkg.WriteTo(_stream.Output.Stream);
                }
                catch (Exception exception)
                {
                    _stream.Output.Reset();
                    _log.Error(string.Format("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\"", channel, req.Serial, _descriptor.FullName, _descriptor.GetRpcServiceCall(req.Call).Name, args, ret), exception);
                    return eRpcException.CalleeRetSerialize;
                }

                try
                {
                    _stream.Output.Stream.Flush();
                }
                catch (Exception exception)
                {
                    _stream.Output.Reset();
                    _log.Error(string.Format("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\"", channel, req.Serial, _descriptor.FullName, _descriptor.GetRpcServiceCall(req.Call).Name, args, ret), exception);
                    return eRpcException.CalleeRetSerialize;
                }

                var buf = _stream.Output.GetResult();

                if (_communicator.Send(channel, buf.Array, buf.Offset, buf.Count))
                {
                    if (_args.LogCall)
                    {
                        _log.InfoFormat("[Rpc]RpcCallee.Call Rsp, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\"", channel, req.Serial, _descriptor.FullName, _descriptor.GetRpcServiceCall(req.Call).Name, args, ret);
                    }
                }
                else
                {
                    _log.ErrorFormat("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\"", channel, req.Serial, _descriptor.FullName, _descriptor.GetRpcServiceCall(req.Call).Name, args, ret);
                    return eRpcException.CalleeCommunicator;
                }

                return eRpcException.None;
            default:
                return e;
            }
        }

        protected void Call<TArgs, TRet>(int channel, RpcCallReq req, Func<int, TArgs, TRet> call) where TArgs : IMessage, new() where TRet : IMessage
        {
            TArgs args;
            TRet ret;
            Call<TArgs, TRet>(channel, req, call, out args, out ret);
        }

        protected void CallAsync<TArgs, TRet>(int channel, RpcCallReq req, Func<int, TArgs, TRet> call) where TArgs : IMessage, new() where TRet : IMessage
        {
            TArgs args;
            TRet ret;
            var e = CallAsync<TArgs, TRet>(channel, req, call, out args, out ret);

            switch (e)
            {
            case eRpcException.None:
            case eRpcException.CalleeCommunicator:
                break;
            default:
                var pkg = new RpcCallExceptionRsp()
                {
                    Serial = req.Serial,
                    Service = req.Service,
                    Call = req.Call,
                    Exception = (int)e,
                };

                _stream.Output.Clear();
                var cmd = BitConverter.GetBytes((int)eRpcProto.RpcCallExceptionRsp);

                try
                {
                    _stream.Output.Buf.Write(cmd, 0, cmd.Length);
                }
                catch (Exception exception)
                {
                    _stream.Output.Reset();
                    _log.Error(string.Format("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\", exception:\"{6}\"", channel, req.Serial, _descriptor.FullName, _descriptor.GetRpcServiceCall(req.Call).Name, args, ret, e), exception);
                    return;
                }

                try
                {
                    pkg.WriteTo(_stream.Output.Stream);
                }
                catch (Exception exception)
                {
                    _stream.Output.Reset();
                    _log.Error(string.Format("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\", exception:\"{6}\"", channel, req.Serial, _descriptor.FullName, _descriptor.GetRpcServiceCall(req.Call).Name, args, ret, e), exception);
                    return;
                }

                try
                {
                    _stream.Output.Stream.Flush();
                }
                catch (Exception exception)
                {
                    _stream.Output.Reset();
                    _log.Error(string.Format("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\", exception:\"{6}\"", channel, req.Serial, _descriptor.FullName, _descriptor.GetRpcServiceCall(req.Call).Name, args, ret, e), exception);
                    return;
                }

                var buf = _stream.Output.GetResult();

                if (_communicator.Send(channel, buf.Array, buf.Offset, buf.Count))
                {
                    if (_args.LogCall)
                    {
                        _log.InfoFormat("[Rpc]RpcCallee.Call ExceptionRsp, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\", exception:\"{6}\"", channel, req.Serial, _descriptor.FullName, _descriptor.GetRpcServiceCall(req.Call).Name, args, ret, e);
                    }
                }
                else
                {
                    _log.ErrorFormat("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\", exception:\"{6}\"", channel, req.Serial, _descriptor.FullName, _descriptor.GetRpcServiceCall(req.Call).Name, args, ret, e);
                }
                break;
            }
        }

        public virtual void Call(int channel, RpcCallReq req)
        {
            _log.ErrorFormat("[Rpc]RpcCallee.Call !Call, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", req:\"{4}\"", channel, req.Serial, _descriptor.FullName, _descriptor.GetRpcServiceCall(req.Call).Name, req);
        }
    }
}
