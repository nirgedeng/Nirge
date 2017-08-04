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

    public interface IRpcService
    {
    }

    #endregion

    #region 

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

    #endregion

    #region 

    public class CRpcInputStream : IDisposable
    {
        CBufStream _buf;
        CodedInputStream _stream;

        public CBufStream Buf
        {
            get
            {
                return _buf;
            }
        }

        public CodedInputStream Stream
        {
            get
            {
                return _stream;
            }
        }

        public CRpcInputStream()
        {
            _buf = new CBufStream(new byte[0], 0, 0);
            _stream = new CodedInputStream(_buf, true);
        }

        public void Clear()
        {
            _buf.Position = 0;
        }

        public void Reset()
        {
            Clear();

            try
            {
                _stream.Dispose();
            }
            catch
            {
            }
            _stream = new CodedInputStream(_buf, true);
        }

        public void Dispose()
        {
            _stream.Dispose();
            _buf.Dispose();
        }
    }

    public class CRpcOutputStream : IDisposable
    {
        CBufStream _buf;
        CodedOutputStream _stream;

        public CBufStream Buf
        {
            get
            {
                return _buf;
            }
        }

        public CodedOutputStream Stream
        {
            get
            {
                return _stream;
            }
        }

        public CRpcOutputStream(byte[] buf, int offset, int count)
        {
            _buf = new CBufStream(buf, offset, count);
            _stream = new CodedOutputStream(_buf, true);
        }

        public ArraySegment<byte> GetResult()
        {
            return new ArraySegment<byte>(_buf.Array, _buf.Offset, _buf.Count);
        }

        public void Clear()
        {
            _buf.Position = 0;
        }

        public void Reset()
        {
            Clear();

            try
            {
                _stream.Dispose();
            }
            catch
            {
            }
            _stream = new CodedOutputStream(_buf, true);
        }

        public void Dispose()
        {
            _stream.Dispose();
            _buf.Dispose();
        }
    }

    public class CRpcStream : IDisposable
    {
        CRpcInputStream _input;
        CRpcOutputStream _output;

        public CRpcInputStream Input
        {
            get
            {
                return _input;
            }
        }

        public CRpcOutputStream Output
        {
            get
            {
                return _output;
            }
        }

        public CRpcStream(CRpcInputStream input, CRpcOutputStream output)
        {
            _input = input;
            _output = output;
        }

        public void Dispose()
        {
            _input.Dispose();
            _output.Dispose();
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
        CallerBreak,
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

    public class CCallerBreakRpcException : CRpcException
    {
        public CCallerBreakRpcException()
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
