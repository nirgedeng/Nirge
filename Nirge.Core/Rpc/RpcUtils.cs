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
    #region 

    public enum eRpcException
    {
        None,
        CallerChannel,
        CallerArgsSerialize,
        CallerRetDeserialize,
        CallerTimeout,
        CalleeArgsDeserialize,
        CalleeExec,
        CalleeRetSerialize,
    }

    public class CRpcException : Exception
    {
        public CRpcException() : base() { }
        public CRpcException(string message) : base(message) { }
        public CRpcException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class CCallerChannelRpcException : CRpcException
    {
        public CCallerChannelRpcException()
        {
        }
    }

    public class CCallerArgsSerializeRpcException : CRpcException
    {
        public CCallerArgsSerializeRpcException()
        {
        }
    }

    public class CCallerRetDeserializeRpcException : CRpcException
    {
        public CCallerRetDeserializeRpcException()
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

    #endregion

    #region 

    public enum eRpcProto
    {
        None,

        RpcCallReq = 1,

        RpcCallRsp = 6,
        RpcCallExceptionRsp,

        Total,
    }

    #endregion

    #region 

    public abstract class CRpcCommunicator
    {
        public abstract bool Send(int channel, byte[] buf, int offset, int count);
    }

    #endregion
}
