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

    public abstract class CRpcCommunicator
    {
        public abstract bool Send(int channel, byte[] buf, int offset, int count);
    }

    #endregion

    #region 

    public interface IRpcService
    {
        void Call(int channel, RpcCallReq req);
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

    public class CRpcStream : IDisposable
    {
        ArraySegment<byte> _buf;
        CBufStream _stream;
        CodedInputStream _inputStream;
        CodedOutputStream _outputStream;

        public CodedInputStream InputStream
        {
            get
            {
                return _inputStream;
            }
        }

        public CodedOutputStream OutputStream
        {
            get
            {
                return _outputStream;
            }
        }

        private CRpcStream(byte[] buf, int offset, int count)
        {
            _buf = new ArraySegment<byte>(buf, offset, count);
            _stream = new CBufStream(buf, offset, count);
            _inputStream = new CodedInputStream(_stream, true);
            _outputStream = new CodedOutputStream(_stream, true);
        }

        public CRpcStream(int capacity)
            :
            this(new byte[capacity], 0, capacity)
        {
        }

        public ArraySegment<byte> GetOutputBuf()
        {
            return new ArraySegment<byte>(_stream.Array, _stream.Offset, _stream.Count);
        }

        public void Clear()
        {
            _stream.Position = 0;
        }

        public void Reset()
        {
            Clear();

            try
            {
                _inputStream.Dispose();
            }
            catch
            {
            }
            _inputStream = new CodedInputStream(_stream, true);

            try
            {
                _outputStream.Dispose();
            }
            catch
            {
            }
            _outputStream = new CodedOutputStream(_stream, true);
        }

        public void Dispose()
        {
            _inputStream.Dispose();
            _outputStream.Dispose();
            _stream.Dispose();
        }
    }

    #endregion

    #region 

    public enum eRpcException
    {
        None,
        CallerCommunicator,
        CallerArgsNull,
        CallerArgsSerialize,
        CCallerReqSerialize,
        CallerRetNull,
        CallerRetDeserialize,
        CallerTimeout,
        CalleeCommunicator,
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

    public class CCallerCommunicatorRpcException : CRpcException
    {
        public CCallerCommunicatorRpcException()
        {
        }
    }

    public class CCallerArgsNullRpcException : CRpcException
    {
        public CCallerArgsNullRpcException()
        {
        }
    }

    public class CCallerArgsSerializeRpcException : CRpcException
    {
        public CCallerArgsSerializeRpcException()
        {
        }
    }

    public class CCCallerReqSerializeRpcException : CRpcException
    {
        public CCCallerReqSerializeRpcException()
        {
        }
    }

    public class CCallerRetNullRpcException : CRpcException
    {
        public CCallerRetNullRpcException()
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

    public class CCalleeCommunicatorRpcException : CRpcException
    {
        public CCalleeCommunicatorRpcException()
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
}
