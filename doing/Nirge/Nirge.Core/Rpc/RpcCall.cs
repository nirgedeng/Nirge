﻿/*------------------------------------------------------------------
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
    public class CClientRpcTransfer : IRpcTransfer
    {
        CTcpClient _cli;

        public CClientRpcTransfer(CTcpClient cli)
        {
            if (cli == null)
                throw new ArgumentNullException(nameof(cli));

            _cli = cli;
        }

        public void Send<T>(int channel, IMessage<T> pkg) where T : IMessage<T>
        {
            if (channel < 0)
                throw new ArgumentOutOfRangeException(nameof(channel));
            if (pkg == null)
                throw new ArgumentNullException(nameof(pkg));

            _cli.Send(pkg);
        }
    }

    public class CServerRpcTransfer : IRpcTransfer
    {
        CTcpServer _ser;

        public CServerRpcTransfer(CTcpServer ser)
        {
            if (ser == null)
                throw new ArgumentNullException(nameof(ser));

            _ser = ser;
        }

        public void Send<T>(int channel, IMessage<T> pkg) where T : IMessage<T>
        {
            if (channel < 0)
                throw new ArgumentOutOfRangeException(nameof(channel));
            if (pkg == null)
                throw new ArgumentNullException(nameof(pkg));

            _ser.Send(channel, pkg);
        }
    }

    public class CRpcStream : IRpcStream
    {
        CArrayStream _inputStream;
        CodedInputStream _codedInputStream;
        CArrayStream _outputStream;
        CodedOutputStream _codedOutputStream;

        public CArrayStream InputStream
        {
            get
            {
                return _inputStream;
            }
        }

        public CodedInputStream CodedInputStream
        {
            get
            {
                return _codedInputStream;
            }
        }

        public CArrayStream OutputStream
        {
            get
            {
                return _outputStream;
            }
        }

        public CodedOutputStream CodedOutputStream
        {
            get
            {
                return _codedOutputStream;
            }
        }

        public CRpcStream(byte[] outputSeg)
        {
            if (outputSeg == null)
                throw new ArgumentNullException(nameof(outputSeg));
            if (outputSeg.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(outputSeg));

            _inputStream = new CArrayStream(0);
            _codedInputStream = new CodedInputStream(_inputStream, true);
            _outputStream = new CArrayStream(outputSeg);
            _codedOutputStream = new CodedOutputStream(_outputStream, true);
        }
    }

    public class CRpcCallStub : IRpcCallStub
    {
        ulong _serial;
        ServiceDescriptor _descriptor;
        int _service;
        int _call;
        DateTime _time;
        TaskCompletionSource<object> _wait;

        public ulong Serial
        {
            get
            {
                return _serial;
            }
        }

        public ServiceDescriptor Descriptor
        {
            get
            {
                return _descriptor;
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

        public TaskCompletionSource<object> Wait
        {
            get
            {
                return _wait;
            }
        }

        public CRpcCallStub(ulong serial, ServiceDescriptor descriptor, int service, int call, DateTime time)
        {
            _serial = serial;
            _descriptor = descriptor;
            _service = service;
            _call = call;
            _time = time;
            _wait = new TaskCompletionSource<object>();
        }
    }

    public class CRpcCallStubProviderArgs
    {
        int _capacity;

        public int Capacity
        {
            get
            {
                return _capacity;
            }
        }

        public CRpcCallStubProviderArgs(int capacity = 0)
        {
            _capacity = capacity;

            if (_capacity < 10240)
                _capacity = 10240;
        }
    }

    public class CRpcCallStubProvider : IRpcCallStubProvider
    {
        CRpcCallStubProviderArgs _args;
        ILog _log;
        CArrayLinkedList<IRpcCallStub> _stubs;
        Dictionary<ulong, IRpcCallStub> _stubsDict;

        public CRpcCallStubProvider(CRpcCallStubProviderArgs args, ILog log)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            _args = args;
            _log = log;

            _stubs = new CArrayLinkedList<IRpcCallStub>(_args.Capacity);
            _stubsDict = new Dictionary<ulong, IRpcCallStub>();
        }

        public IRpcCallStub CreateStub(ulong serial, ServiceDescriptor descriptor, int service, int call, TimeSpan timeout)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            if (service < 0)
                throw new ArgumentOutOfRangeException(nameof(service));
            if (call < 0)
                throw new ArgumentOutOfRangeException(nameof(call));
            if (timeout == null)
                throw new ArgumentNullException(nameof(timeout));

            var stub = new CRpcCallStub(serial, descriptor, service, call, DateTime.Now.Add(timeout));
            _stubs.AddLast(stub);
            _stubsDict.Add(serial, stub);
            return stub;
        }

        public bool TryGetStub(ulong serial, out IRpcCallStub stub)
        {
            return _stubsDict.TryGetValue(serial, out stub);
        }

        public void DelStub(IRpcCallStub stub)
        {
            if (stub == null)
                throw new ArgumentNullException(nameof(stub));

            if (_stubsDict.Remove(stub.Serial))
                _stubs.Remove(stub);

            throw new ArgumentOutOfRangeException(nameof(stub));
        }

        public void Break()
        {
            foreach (var i in _stubs)
            {
                DelStub(i);

                _log.WriteLine(eLogPattern.Info, $"RPC stub Break serial {i.Serial} service {i.Service} {i.Descriptor.FullName} call {i.Call} {i.Descriptor.GetCall(i.Call).Name} time {i.Time}");

                try
                {
                    i.Wait.SetException(new CCallerBreakRpcException());
                }
                catch (Exception exception)
                {
                    _log.WriteLine(eLogPattern.Error, $"RPC stub Break exception serial {i.Serial} service {i.Service} {i.Descriptor.FullName} call {i.Call} {i.Descriptor.GetCall(i.Call).Name} time {i.Time}", exception);
                }
            }
        }

        public void Exec()
        {
            foreach (var i in _stubs)
            {
                if (DateTime.Now < i.Time)
                    continue;

                DelStub(i);

                _log.WriteLine(eLogPattern.Warn, $"RPC stub timeout serial {i.Serial} service {i.Service} {i.Descriptor.FullName} call {i.Call} {i.Descriptor.GetCall(i.Call).Name} time {i.Time}");

                try
                {
                    i.Wait.SetException(new CCallerBreakRpcException());
                }
                catch (Exception exception)
                {
                    _log.WriteLine(eLogPattern.Error, $"RPC stub timeout exception serial {i.Serial} service {i.Service} {i.Descriptor.FullName} call {i.Call} {i.Descriptor.GetCall(i.Call).Name} time {i.Time}", exception);
                }
            }
        }

        public void Exec(RpcCallRsp rsp)
        {
            if (rsp == null)
                throw new ArgumentNullException(nameof(rsp));

            if (!TryGetStub(rsp.Serial, out var stub))
            {
                _log.WriteLine(eLogPattern.Warn, $"RPC stub rsp disappear serial {rsp.Serial} service {rsp.Service} call {rsp.Call} ret {rsp.Ret.Length}");
                return;
            }

            DelStub(stub);

            try
            {
                stub.Wait.SetResult(rsp.Ret);
            }
            catch (Exception exception)
            {
                _log.WriteLine(eLogPattern.Error, $"RPC stub rsp exception serial {stub.Serial} service {stub.Service} {stub.Descriptor.FullName} call {stub.Call} {stub.Descriptor.GetCall(stub.Call).Name} time {stub.Time} ret {rsp.Ret.Length}", exception);
            }
        }

        public void Exec(RpcCallExceptionRsp rsp)
        {
            if (rsp == null)
                throw new ArgumentNullException(nameof(rsp));

            if (!TryGetStub(rsp.Serial, out var stub))
            {
                _log.WriteLine(eLogPattern.Warn, $"RPC stub exceptionRsp disappear serial {rsp.Serial} service {rsp.Service} call {rsp.Call} exception {rsp.Exception}");
                return;
            }

            DelStub(stub);

            _log.WriteLine(eLogPattern.Info, $"RPC stub exceptionRsp serial {stub.Serial} service {stub.Service} {stub.Descriptor.FullName} call {stub.Call} {stub.Descriptor.GetCall(stub.Call).Name} time {stub.Time} exception {rsp.Exception}");

            try
            {
            }
            catch (Exception exception)
            {
                _log.WriteLine(eLogPattern.Error, $"RPC stub exceptionRsp exception serial {stub.Serial} service {stub.Service} {stub.Descriptor.FullName} call {stub.Call} {stub.Descriptor.GetCall(stub.Call).Name} time {stub.Time} exception {rsp.Exception}", exception);
            }
        }
    }
}
