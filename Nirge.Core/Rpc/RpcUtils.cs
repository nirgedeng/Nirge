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
        MemoryStream _stream;
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
            _stream = new MemoryStream(buf, offset, count);
            _inputStream = new CodedInputStream(_stream, true);
            _outputStream = new CodedOutputStream(_stream, true);
        }

        public CRpcStream(int capacity)
            :
            this(new byte[capacity], 0, capacity)
        {
        }

        public ArraySegment<byte> GetInputBuf()
        {
            return new ArraySegment<byte>(_buf.Array, _buf.Offset, (int)_inputStream.Position);
        }

        public ArraySegment<byte> GetOutputBuf()
        {
            return new ArraySegment<byte>(_buf.Array, _buf.Offset, (int)_outputStream.Position);
        }

        public void Reset()
        {
            _stream.Position = 0;
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

    public class CCallerCommunicatorRpcException : CRpcException
    {
        public CCallerCommunicatorRpcException()
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
}
