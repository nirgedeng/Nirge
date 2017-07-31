/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class CRpcCallerArgs
    {
        int _timeout;
        bool _logCall;

        public int Timeout
        {
            get
            {
                return _timeout;
            }
        }

        public bool LogCall
        {
            get
            {
                return _logCall;
            }
        }

        public CRpcCallerArgs(int timeout, bool logCall)
        {
            _timeout = timeout;
            _logCall = logCall;
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

        protected void Call(int channel, int service, int call)
        {
            var pkg = new RpcCallReq()
            {
                Service = service,
                Call = call,
            };

            _stream.Reset();

            try
            {
                pkg.WriteTo(_stream.OutputStream);
            }
            catch (Exception exception)
            {
                _log.Error(string.Format("[Rpc]RpcCaller.Call exception, channel:\"{0}\", service:\"{1}\", call:\"{2}\"", channel, service, call), exception);
                throw new CCCallerReqSerializeRpcException();
            }

            var buf = _stream.GetOutputBuf();

            if (_communicator.Send(channel, buf.Array, buf.Offset, buf.Count))
            {
                if (_args.LogCall)
                {
                    _log.InfoFormat("[Rpc]RpcCaller.Call , channel:\"{0}\", service:\"{1}\", call:\"{2}\", pkg:\"{3}\"", channel, service, call, pkg);
                }
            }
            else
            {
                _log.ErrorFormat("[Rpc]RpcCaller.Call exception, channel:\"{0}\", service:\"{1}\", call:\"{2}\", call:\"{2}\", pkg:\"{3}\"", channel, service, call, pkg);
                throw new CCallerCommunicatorRpcException();
            }
        }

        protected void Call<TArgs>(int channel, int service, int call, TArgs args) where TArgs : IMessage
        {
            if (args == null)
                throw new CCallerArgsNullRpcException();

            var pkg = new RpcCallReq()
            {
                Service = service,
                Call = call,
            };

            try
            {
                pkg.Args = Google.Protobuf.WellKnownTypes.Any.Pack(args);
            }
            catch (Exception exception)
            {
                _log.Error(string.Format("[Rpc]RpcCaller.Call exception, channel:\"{0}\", service:\"{1}\", call:\"{2}\", args:\"{3}\"", channel, service, call, args), exception);
                throw new CCallerArgsSerializeRpcException();
            }

            _stream.Reset();

            try
            {
                pkg.WriteTo(_stream.OutputStream);
            }
            catch (Exception exception)
            {
                _log.Error(string.Format("[Rpc]RpcCaller.Call exception, channel:\"{0}\", service:\"{1}\", call:\"{2}\", args:\"{3}\"", channel, service, call, args), exception);
                throw new CCCallerReqSerializeRpcException();
            }

            var buf = _stream.GetOutputBuf();

            if (_communicator.Send(channel, buf.Array, buf.Offset, buf.Count))
            {
                if (_args.LogCall)
                {
                    _log.InfoFormat("[Rpc]RpcCaller.Call , channel:\"{0}\", service:\"{1}\", call:\"{2}\", pkg:\"{3}\"", channel, service, call, pkg);
                }
            }
            else
            {
                _log.ErrorFormat("[Rpc]RpcCaller.Call exception, channel:\"{0}\", service:\"{1}\", call:\"{2}\", pkg:\"{3}\"", channel, service, call, pkg);
                throw new CCallerCommunicatorRpcException();
            }
        }

        protected Task<TRet> CallAsync<TArgs, TRet>(int channel, int service, int call, TArgs args) where TArgs : IMessage where TRet : IMessage, new()
        {
            var task = CallAsync<TArgs>(channel, service, call, args);
            if (task == null)
                throw new CCallerRetNullRpcException();
            if (task.Result == null)
                throw new CCallerRetNullRpcException();

            TRet ret;
            try
            {
                ret = task.Result.Unpack<TRet>();
            }
            catch (Exception exception)
            {
                _log.Error(string.Format("[Rpc]RpcCaller.Call exception, channel:\"{0}\", service:\"{1}\", call:\"{2}\", args:\"{3}\"", channel, service, call, args), exception);
                throw new CCallerRetDeserializeRpcException();
            }

            if (_args.LogCall)
            {
                _log.InfoFormat("[Rpc]RpcCaller.Call Rsp, channel:\"{0}\", service:\"{1}\", call:\"{2}\", args:\"{3}\", ret:\"{4}\"", channel, service, call, args, ret);
            }

            return Task.FromResult(ret);
        }

        async Task<Google.Protobuf.WellKnownTypes.Any> CallAsync<TArgs>(int channel, int service, int call, TArgs args) where TArgs : IMessage
        {
            if (args == null)
                throw new CCallerArgsNullRpcException();

            var pkg = new RpcCallReq()
            {
                Service = service,
                Call = call,
            };

            try
            {
                pkg.Args = Google.Protobuf.WellKnownTypes.Any.Pack(args);
            }
            catch (Exception exception)
            {
                _log.Error(string.Format("[Rpc]RpcCaller.Call exception, channel:\"{0}\", service:\"{1}\", call:\"{2}\", args:\"{3}\"", channel, service, call, args), exception);
                throw new CCallerArgsSerializeRpcException();
            }

            var serial = _stubs.CreateSerial();
            pkg.Serial = serial;

            _stream.Reset();

            try
            {
                pkg.WriteTo(_stream.OutputStream);
            }
            catch (Exception exception)
            {
                _log.Error(string.Format("[Rpc]RpcCaller.Call exception, channel:\"{0}\", service:\"{1}\", call:\"{2}\", args:\"{3}\"", channel, service, call, args), exception);
                throw new CCCallerReqSerializeRpcException();
            }

            var buf = _stream.GetOutputBuf();

            if (_communicator.Send(channel, buf.Array, buf.Offset, buf.Count))
            {
                if (_args.LogCall)
                {
                    _log.InfoFormat("[Rpc]RpcCaller.Call Req, channel:\"{0}\", service:\"{1}\", call:\"{2}\", pkg:\"{3}\"", channel, service, call, pkg);
                }
            }
            else
            {
                _log.ErrorFormat("[Rpc]RpcCaller.Call exception, channel:\"{0}\", service:\"{1}\", call:\"{2}\", pkg:\"{3}\"", channel, service, call, pkg);
                throw new CCallerCommunicatorRpcException();
            }

            var stub = _stubs.CreateStub(serial, service, call, _args.Timeout);

            var task = stub.Awaiter.Task;
            try
            {
                await task;
            }
            catch (CRpcException exception)
            {
                _log.Error(string.Format("[Rpc]RpcCaller.Call exception, channel:\"{0}\", service:\"{1}\", call:\"{2}\", args:\"{3}\"", channel, service, call, args), exception);
                throw exception;
            }
            catch (Exception exception)
            {
                _log.Error(string.Format("[Rpc]RpcCaller.Call exception, channel:\"{0}\", service:\"{1}\", call:\"{2}\", args:\"{3}\"", channel, service, call, args), exception);
                throw new CRpcException("", exception);
            }
            return task.Result;
        }
    }
}
