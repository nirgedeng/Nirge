/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Reflection;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using System;

namespace Nirge.Core
{
    public enum RpcCallException
    {
        None,
        CalleeArgsDeserialize,
        CalleeExec,
        CalleeRetSerialize,
    }

    public class RpcException : Exception
    {
        public RpcException() : base() { }
        public RpcException(string message) : base(message) { }
        public RpcException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class CallerChannelRpcException : RpcException
    {
        public CallerChannelRpcException()
        {
        }
    }

    public class CallerArgsSerializeRpcException : RpcException
    {
        public CallerArgsSerializeRpcException()
        {
        }
    }

    public class CallerRetDeserializeRpcException : RpcException
    {
        public CallerRetDeserializeRpcException()
        {
        }
    }

    public class CallerTimeoutRpcException : RpcException
    {
        public CallerTimeoutRpcException()
        {
        }
    }

    public class CalleeArgsDeserializeRpcException : RpcException
    {
        public CalleeArgsDeserializeRpcException()
        {
        }
    }

    public class CalleeExecRpcException : RpcException
    {
        public CalleeExecRpcException()
        {
        }
    }

    public class CalleeRetSerializeRpcException : RpcException
    {
        public CalleeRetSerializeRpcException()
        {
        }
    }

    public enum RpcProto
    {
        C2sRpcCall = 1,

        S2cRpcCall = 11,
        S2cRpcCallException,
    }
}
