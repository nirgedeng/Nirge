/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

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

    public class CRpcCallee<TRpcService> where TRpcService : IRpcService
    {
        static RpcCallArgsEmpty ArgsEmpty = new RpcCallArgsEmpty();

        CRpcCalleeArgs _args;
        ILog _log;
        CRpcStream _stream;
        CRpcCommunicator _communicator;
        protected TRpcService _service;

        public CRpcCallee(CRpcCalleeArgs args, ILog log, CRpcStream stream, CRpcCommunicator communicator, TRpcService service)
        {
            _args = args;
            _log = log;
            _stream = stream;
            _communicator = communicator;
            _service = service;
        }

        eRpcException Call<TArgs, TRet>(int channel, RpcCallReq req, Func<int, TArgs, TRet> call, out TArgs args, out TRet ret) where TArgs : IMessage, new() where TRet : IMessage
        {
            args = default(TArgs);
            ret = default(TRet);

            try
            {
                args = req.Args.Unpack<TArgs>();
            }
            catch (Exception exception)
            {
                _log.Error(string.Format("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\"", channel, req.Serial, req.Service, req.Call), exception);
                return eRpcException.CalleeArgsDeserialize;
            }

            try
            {
                ret = call(channel, args);
            }
            catch (Exception exception)
            {
                _log.Error(string.Format("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\"", channel, req.Serial, req.Service, req.Call, args), exception);
                return eRpcException.CalleeExec;
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
                    pkg.Ret = Google.Protobuf.WellKnownTypes.Any.Pack(ret);
                }
                catch (Exception exception)
                {
                    _log.Error(string.Format("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\"", channel, req.Serial, req.Service, req.Call, args, ret), exception);
                    return eRpcException.CalleeRetSerialize;
                }

                _stream.Clear();

                try
                {
                    pkg.WriteTo(_stream.OutputStream);
                }
                catch (Exception exception)
                {
                    _log.Error(string.Format("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\"", channel, req.Serial, req.Service, req.Call, args, ret), exception);
                    return eRpcException.CalleeRetSerialize;
                }

                try
                {
                    _stream.OutputStream.Flush();
                }
                catch (Exception exception)
                {
                    _stream.Reset();
                    _log.Error(string.Format("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\"", channel, req.Serial, req.Service, req.Call, args, ret), exception);
                    return eRpcException.CalleeRetSerialize;
                }

                var buf = _stream.GetOutputBuf();

                if (_communicator.Send(channel, buf.Array, buf.Offset, buf.Count))
                {
                    if (_args.LogCall)
                    {
                        _log.InfoFormat("[Rpc]RpcCallee.Call Rsp, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\"", channel, req.Serial, req.Service, req.Call, args, ret);
                    }
                }
                else
                {
                    _log.ErrorFormat("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\"", channel, req.Serial, req.Service, req.Call, args, ret);
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

                _stream.Clear();

                try
                {
                    pkg.WriteTo(_stream.OutputStream);
                }
                catch (Exception exception)
                {
                    _log.Error(string.Format("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\", exception:\"{6}\"", channel, req.Serial, req.Service, req.Call, args, ret, pkg), exception);
                    return;
                }

                try
                {
                    _stream.OutputStream.Flush();
                }
                catch (Exception exception)
                {
                    _stream.Reset();
                    _log.Error(string.Format("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\", exception:\"{6}\"", channel, req.Serial, req.Service, req.Call, args, ret, pkg), exception);
                    return;
                }

                var buf = _stream.GetOutputBuf();

                if (_communicator.Send(channel, buf.Array, buf.Offset, buf.Count))
                {
                    if (_args.LogCall)
                    {
                        _log.InfoFormat("[Rpc]RpcCallee.Call Rsp, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\", exception:\"{6}\"", channel, req.Serial, req.Service, req.Call, args, ret, pkg);
                    }
                }
                else
                {
                    _log.ErrorFormat("[Rpc]RpcCallee.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2}\", call:\"{3}\", args:\"{4}\", ret:\"{5}\", exception:\"{6}\"", channel, req.Serial, req.Service, req.Call, args, ret, pkg);
                }
                break;
            }
        }

        public virtual void Call(int channel, RpcCallReq req)
        {
        }
    }
}
