﻿/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using Google.Protobuf.Reflection;
using System.Threading.Tasks;
using System.Threading;
using Google.Protobuf;
using log4net;
using System;

namespace Nirge.Core
{
    #region 

    public class CRpcCallerArgs
    {
        TimeSpan _timeout;
        bool _logCall;

        public TimeSpan Timeout
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

        public CRpcCallerArgs(TimeSpan timeout, bool logCall)
        {
            _timeout = timeout;
            _logCall = logCall;
        }
    }

    #endregion

    public abstract class CRpcCaller
    {
        protected static RpcCallArgsEmpty ArgsEmpty = new RpcCallArgsEmpty();

        CRpcCallerArgs _args;
        ILog _log;
        CRpcStream _stream;
        CRpcCommunicator _communicator;
        CRpcCallStubProvider _stubs;
        ServiceDescriptor _descriptor;
        protected int _service;

        public CRpcCaller(CRpcCallerArgs args, ILog log, CRpcStream stream, CRpcCommunicator communicator, CRpcCallStubProvider stubs, ServiceDescriptor descriptor, int service)
        {
            _args = args;
            _log = log;
            _stream = stream;
            _communicator = communicator;
            _stubs = stubs;
            _descriptor = descriptor;
            _service = service;
        }

        void Call<TArgs>(int channel, int serial, int call, TArgs args, RpcCallReq pkg) where TArgs : IMessage
        {
            try
            {
                pkg.Args = args.ToByteString();
            }
            catch (Exception exception)
            {
                _log.Error(string.Format("[Rpc]RpcCaller.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2},{3}\", call:\"{4},{5}\", args:\"{6}\""
                    , channel
                    , serial
                    , _service
                    , _descriptor.FullName
                    , call
                    , _descriptor.GetRpcServiceCall(call).Name, args), exception);
                throw new CCallerArgsSerializeRpcException();
            }

            _stream.Output.Clear();
            var cmd = BitConverter.GetBytes((int)eRpcProto.RpcCallReq);

            try
            {
                _stream.Output.Buf.Write(cmd, 0, cmd.Length);
            }
            catch (Exception exception)
            {
                _stream.Output.Reset();
                _log.Error(string.Format("[Rpc]RpcCaller.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2},{3}\", call:\"{4},{5}\", args:\"{6}\""
                    , channel
                    , serial
                    , _service
                    , _descriptor.FullName
                    , call
                    , _descriptor.GetRpcServiceCall(call).Name
                    , args), exception);
                throw new CCCallerReqSerializeRpcException();
            }

            try
            {
                pkg.WriteTo(_stream.Output.Stream);
            }
            catch (Exception exception)
            {
                _stream.Output.Reset();
                _log.Error(string.Format("[Rpc]RpcCaller.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2},{3}\", call:\"{4},{5}\", args:\"{6}\""
                    , channel
                    , serial
                    , _service
                    , _descriptor.FullName
                    , call
                    , _descriptor.GetRpcServiceCall(call).Name
                    , args), exception);
                throw new CCCallerReqSerializeRpcException();
            }

            try
            {
                _stream.Output.Stream.Flush();
            }
            catch (Exception exception)
            {
                _stream.Output.Reset();
                _log.Error(string.Format("[Rpc]RpcCaller.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2},{3}\", call:\"{4},{5}\", args:\"{6}\""
                    , channel
                    , serial
                    , _service
                    , _descriptor.FullName
                    , call
                    , _descriptor.GetRpcServiceCall(call).Name
                    , args), exception);
                throw new CCCallerReqSerializeRpcException();
            }

            var buf = _stream.Output.GetResult();

            if (_communicator.Send(channel, buf.Array, buf.Offset, buf.Count))
            {
                if (_args.LogCall)
                {
                    _log.InfoFormat("[Rpc]RpcCaller.Call Req, channel:\"{0}\", serial:\"{1}\", service:\"{2},{3}\", call:\"{4},{5}\", args:\"{6}\""
                        , channel
                        , serial
                        , _service
                        , _descriptor.FullName
                        , call
                        , _descriptor.GetRpcServiceCall(call).Name
                        , args);
                }
            }
            else
            {
                _log.ErrorFormat("[Rpc]RpcCaller.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2},{3}\", call:\"{4},{5}\", args:\"{6}\""
                    , channel
                    , serial
                    , _service
                    , _descriptor.FullName
                    , call
                    , _descriptor.GetRpcServiceCall(call).Name
                    , args);
                throw new CCallerCommunicatorRpcException();
            }
        }

        async Task<TRet> CallAsync<TArgs, TRet>(int channel, int serial, int call, TArgs args, RpcCallReq pkg) where TArgs : IMessage where TRet : IMessage, new()
        {
            var stub = _stubs.CreateStub(serial, _descriptor, _service, call, _args.Timeout);
            var task = stub.Awaiter.Task;

            try
            {
                await task;
            }
            catch (CRpcException exception)
            {
                _log.Error(string.Format("[Rpc]RpcCaller.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2},{3}\", call:\"{4},{5}\", args:\"{6}\""
                    , channel
                    , serial
                    , _service
                    , _descriptor.FullName
                    , call
                    , _descriptor.GetRpcServiceCall(call).Name
                    , args), exception);
                throw exception;
            }
            catch (Exception exception)
            {
                _log.Error(string.Format("[Rpc]RpcCaller.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2},{3}\", call:\"{4},{5}\", args:\"{6}\""
                    , channel
                    , serial
                    , _service
                    , call
                    , _descriptor.FullName, _descriptor.GetRpcServiceCall(call).Name
                    , args), exception);
                throw new CRpcException("", exception);
            }

            if (task.Result == null)
                throw new CCallerRetNullRpcException();

            var ret = new TRet();
            try
            {
                ret.MergeFrom(task.Result);
            }
            catch (Exception exception)
            {
                _log.Error(string.Format("[Rpc]RpcCaller.Call exception, channel:\"{0}\", serial:\"{1}\", service:\"{2},{3}\", call:\"{4},{5}\", args:\"{6}\""
                    , channel
                    , serial
                    , _service
                    , _descriptor.FullName
                    , call
                    , _descriptor.GetRpcServiceCall(call).Name
                    , args), exception);
                throw new CCallerRetDeserializeRpcException();
            }

            if (_args.LogCall)
            {
                _log.InfoFormat("[Rpc]RpcCaller.Call Rsp, channel:\"{0}\", serial:\"{1}\", service:\"{2},{3}\", call:\"{4},{5}\", args:\"{6}\", ret:\"{7}\""
                    , channel
                    , serial
                    , _service
                    , _descriptor.FullName
                    , call
                    , _descriptor.GetRpcServiceCall(call).Name
                    , args
                    , ret);
            }

            return ret;
        }

        protected void Call<TArgs>(int channel, int call, TArgs args) where TArgs : IMessage
        {
            if (args == null)
                throw new CCallerArgsNullRpcException();

            var pkg = new RpcCallReq()
            {
                Service = _service,
                Call = call,
            };

            Call<TArgs>(channel, 0, call, args, pkg);
        }

        protected Task<TRet> CallAsync<TArgs, TRet>(int channel, int call, TArgs args) where TArgs : IMessage where TRet : IMessage, new()
        {
            if (args == null)
                throw new CCallerArgsNullRpcException();

            var pkg = new RpcCallReq()
            {
                Service = _service,
                Call = call,
            };

            var serial = _stubs.CreateSerial();
            pkg.Serial = serial;

            Call<TArgs>(channel, serial, call, args, pkg);

            return CallAsync<TArgs, TRet>(channel, serial, call, args, pkg);
        }
    }
}
