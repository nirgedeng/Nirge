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
    public class RpcException : Exception
    {
        public RpcException() : base() { }
        public RpcException(string message) : base(message) { }
        public RpcException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ChannelRpcException : RpcException
    {
        public ChannelRpcException()
        {
        }
    }
    public class CalleeRpcException : RpcException
    {
        public CalleeRpcException()
        {
        }
    }

    public class TimeoutRpcException : RpcException
    {
        public TimeoutRpcException()
        {
        }
    }
}
