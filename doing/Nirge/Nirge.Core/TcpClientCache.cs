/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System;

namespace Nirge.Core
{
    #region 

    public class CTcpClientCacheArgs
    {
        int _recvBufSize;
        int _sendCacheSize;
        int _recvCacheSize;

        public int RecvBufSize
        {
            get
            {
                return _recvBufSize;
            }
        }

        public int SendCacheSize
        {
            get
            {
                return _sendCacheSize;
            }
        }

        public int RecvCacheSize
        {
            get
            {
                return _recvCacheSize;
            }
        }

        public CTcpClientCacheArgs(int recvBufSize = 0, int sendCacheSize = 0, int recvCacheSize = 0)
        {
            _recvBufSize = recvBufSize;
            _sendCacheSize = sendCacheSize;
            _recvCacheSize = recvCacheSize;

            if (_recvBufSize < 8192)
                _recvBufSize = 8192;
            if (_sendCacheSize == 0)
                _sendCacheSize = 1073741824;
            if (_recvCacheSize == 0)
                _recvCacheSize = 1073741824;
        }
    }

    #endregion

    public class CTcpClientCacheEmpty : ITcpClientCache
    {
        CTcpClientCacheArgs _args;

        public CTcpClientCacheEmpty(CTcpClientCacheArgs args)
        {
            _args = args;
        }

        public void Clear()
        {
        }

        #region 

        public eTcpError AllocSendBuf(int count, out byte[] buf)
        {
            buf = new byte[count];
            return eTcpError.None;
        }

        public eTcpError CollectSendBuf(byte[] buf)
        {
            return eTcpError.None;
        }

        #endregion

        #region 

        public eTcpError AllocRecvBuf(out byte[] buf)
        {
            buf = new byte[_args.RecvBufSize];
            return eTcpError.None;
        }

        public eTcpError CollectRecvBuf(byte[] buf)
        {
            return eTcpError.None;
        }

        #endregion
    }

    public class CTcpClientCache : ITcpClientCache
    {
        static int[] gTcpClientBufSize = { 32, 64, 128, 256, 512, 1024, 2048 };

        CTcpClientCacheArgs _args;

        ConcurrentQueue<byte[]>[] _sends;
        ConcurrentQueue<byte[]> _recvs;

        public CTcpClientCache(CTcpClientCacheArgs args)
        {
            _args = args;

            _sends = new ConcurrentQueue<byte[]>[gTcpClientBufSize.Length];
            _recvs = new ConcurrentQueue<byte[]>();
        }

        public void Clear()
        {
            byte[] buf;

            foreach (var i in _sends)
            {
                while (i.Count > 0)
                    i.TryDequeue(out buf);
            }

            while (_recvs.Count > 0)
                _recvs.TryDequeue(out buf);
        }

        #region 

        public eTcpError AllocSendBuf(int count, out byte[] buf)
        {
            if (count == 0)
            {
                buf = null;
                return eTcpError.PkgSizeIsZero;
            }

            for (var i = 0; i < gTcpClientBufSize.Length; ++i)
            {
                if (count < gTcpClientBufSize[i])
                {
                    if (!_sends[i].TryDequeue(out buf))
                    {
                        buf = new byte[gTcpClientBufSize[i]];
                        return eTcpError.None;
                    }
                }
            }

            buf = null;
            return eTcpError.PkgSizeOutOfRange;
        }

        public eTcpError CollectSendBuf(byte[] buf)
        {
            if (buf == null)
                return eTcpError.ArgumentNull;

            for (var i = 0; i < gTcpClientBufSize.Length; ++i)
            {
                if (buf.Length == gTcpClientBufSize[i])
                {
                    _sends[i].Enqueue(buf);
                    return eTcpError.None;
                }
            }

            return eTcpError.PkgSizeOutOfRange;
        }

        #endregion

        #region 

        public eTcpError AllocRecvBuf(out byte[] buf)
        {
            if (!_recvs.TryDequeue(out buf))
                buf = new byte[_args.RecvBufSize];
            return eTcpError.None;
        }

        public eTcpError CollectRecvBuf(byte[] buf)
        {
            if (buf == null)
                return eTcpError.ArgumentNull;
            if (buf.Length != _args.RecvBufSize)
                return eTcpError.ArgumentOutOfRange;

            _recvs.Enqueue(buf);
            return eTcpError.None;
        }

        #endregion
    }
}
