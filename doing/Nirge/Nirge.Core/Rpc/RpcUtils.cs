/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace Nirge.Core
{
    public static class CRpcUtils
    {
        public const int
            gRpcServiceOption = 60101,
            gRpcServiceCallOption = 60201
            ;

        public static MethodDescriptor GetCall(this ServiceDescriptor descriptor, int call)
        {
            return descriptor.Methods.FirstOrDefault(i =>
            {
                return i.CustomOptions.TryGetMessage<RpcServiceCallOption>(gRpcServiceCallOption, out var option)
                        && call == option.Uid;
            });
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CRpcServiceAttribute : Attribute
    {
        int _uid;

        public int Uid
        {
            get
            {
                return _uid;
            }
        }

        public CRpcServiceAttribute(int uid)
        {
            _uid = uid;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CRpcServiceCallAttribute : Attribute
    {
        int _uid;
        bool _isOneWay;

        public int Uid
        {
            get
            {
                return _uid;
            }
        }

        public bool IsOneWay
        {
            get
            {
                return _isOneWay;
            }
        }

        public CRpcServiceCallAttribute(int uid, bool isOneWay)
        {
            _uid = uid;
            _isOneWay = isOneWay;
        }
    }

    public interface IRpcService
    {
    }

    public interface IRpcTransfer
    {
        void Send<T>(int channel, IMessage<T> pkg) where T : IMessage<T>;
    }

    public interface IRpcStream
    {
        CArrayStream InputStream
        {
            get;
        }

        CodedInputStream CodedInputStream
        {
            get;
        }

        CArrayStream OutputStream
        {
            get;
        }

        CodedOutputStream CodedOutputStream
        {
            get;
        }
    }

    public interface IRpcCallStub
    {
        ulong Serial
        {
            get;
        }

        ServiceDescriptor Descriptor
        {
            get;
        }

        int Service
        {
            get;
        }

        int Call
        {
            get;
        }

        DateTime Time
        {
            get;
        }

        TaskCompletionSource<ByteString> Wait
        {
            get;
        }
    }

    public interface IRpcCallStubProvider
    {
        int Count
        {
            get;
        }

        bool IsFull
        {
            get;
        }

        IRpcCallStub CreateStub(ServiceDescriptor descriptor, int service, int call, TimeSpan timeout);
        bool TryGetStub(ulong serial, out IRpcCallStub stub);
        void DelStub(IRpcCallStub stub);
        void Break();
        void Exec();
        void Exec(RpcCallRsp rsp);
        void Exec(RpcCallExceptionRsp rsp);
    }

    public enum eRpcException
    {
        None,
        CalleeTransfer,
        CalleeArgsDeserialize,
        CalleeExec,
        CalleeRetSerialize,
        CalleeInvalid,
    }

    public class CRpcException : Exception
    {
        public CRpcException()
            :
            base()
        {
        }

        public CRpcException(string message)
            :
            base(message)
        {
        }

        public CRpcException(string message, Exception innerException)
            :
            base(message, innerException)
        {
        }
    }

    public class CCallerBreakRpcException : CRpcException
    {
        public CCallerBreakRpcException()
        {
        }
    }

    public class CCallerTimeoutRpcException : CRpcException
    {
        public CCallerTimeoutRpcException()
        {
        }
    }

    public class CCalleeArgsDeserializeRpcException : CRpcException
    {
        public CCalleeArgsDeserializeRpcException()
        {
        }
    }

    public class CCalleeExecRpcException : CRpcException
    {
        public CCalleeExecRpcException()
        {
        }
    }

    public class CCalleeRetSerializeRpcException : CRpcException
    {
        public CCalleeRetSerializeRpcException()
        {
        }
    }

    public class CCalleeInvalidRpcException : CRpcException
    {
        public CCalleeInvalidRpcException()
        {
        }
    }
}
