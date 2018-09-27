/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System;
using System.Threading;

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

    public class CTcpClientCache : ITcpClientCache
    {
        static readonly int[] gTcpClientBufSize = { 32, 64, 128, 256, 512, 1024, 2048 };

        CTcpClientCacheArgs _args;

        int _sendCacheSize;
        int _sendCacheSizeAlloc;
        ConcurrentQueue<byte[]>[] _sends;
        int _recvCacheSize;
        int _recvCacheSizeAlloc;
        ConcurrentQueue<byte[]> _recvs;

        public CTcpClientCacheArgs Args
        {
            get
            {
                return _args;
            }
        }

        public int SendCacheSize
        {
            get
            {
                return _sendCacheSize;
            }
        }

        public int SendCacheSizeAlloc
        {
            get
            {
                return _sendCacheSizeAlloc;
            }
        }

        public int RecvBufSize
        {
            get
            {
                return _args.RecvBufSize;
            }
        }

        public int RecvCacheSize
        {
            get
            {
                return _recvCacheSize;
            }
        }

        public int RecvCacheSizeAlloc
        {
            get
            {
                return _recvCacheSizeAlloc;
            }
        }

        public bool CanAllocSendBuf
        {
            get
            {
                if (_sendCacheSizeAlloc > _args.SendCacheSize)
                    return false;
                return true;
            }
        }

        public bool CanAllocRecvBuf
        {
            get
            {
                if (_recvCacheSizeAlloc > _args.RecvCacheSize)
                    return false;
                return true;
            }
        }

        public CTcpClientCache(CTcpClientCacheArgs args)
        {
            _args = args;

            _sends = new ConcurrentQueue<byte[]>[gTcpClientBufSize.Length];
            for (var i = 0; i < _sends.Length; ++i)
                _sends[i] = new ConcurrentQueue<byte[]>();
            _recvs = new ConcurrentQueue<byte[]>();

            Clear();
        }

        public void Clear()
        {
            byte[] buf;

            _sendCacheSize = 0;
            _sendCacheSizeAlloc = 0;
            foreach (var i in _sends)
            {
                while (i.Count > 0)
                    i.TryDequeue(out buf);
            }

            _recvCacheSize = 0;
            _recvCacheSizeAlloc = 0;
            while (_recvs.Count > 0)
                _recvs.TryDequeue(out buf);
        }

        #region 

        public eTcpError AllocSendBuf(int count, out byte[] buf)
        {
            if (count == 0)
            {
                buf = null;
                return eTcpError.BlockSizeIsZero;
            }

            for (var i = 0; i < gTcpClientBufSize.Length; ++i)
            {
                if (count > gTcpClientBufSize[i])
                    continue;

                if (_sends[i].TryDequeue(out buf))
                    Interlocked.Add(ref _sendCacheSize, -buf.Length);
                else
                    buf = new byte[gTcpClientBufSize[i]];
                Interlocked.Add(ref _sendCacheSizeAlloc, buf.Length);
                return eTcpError.Success;
            }

            buf = null;
            return eTcpError.BlockSizeOutOfRange;
        }

        public eTcpError CollectSendBuf(byte[] buf)
        {
            if (buf == null)
                return eTcpError.BlockNull;
            if (buf.Length == 0)
                return eTcpError.BlockSizeIsZero;

            for (var i = 0; i < gTcpClientBufSize.Length; ++i)
            {
                if (buf.Length == gTcpClientBufSize[i])
                {
                    _sends[i].Enqueue(buf);
                    Interlocked.Add(ref _sendCacheSize, buf.Length);
                    Interlocked.Add(ref _sendCacheSizeAlloc, -buf.Length);
                    return eTcpError.Success;
                }
            }

            return eTcpError.BlockSizeOutOfRange;
        }

        #endregion

        #region 

        public eTcpError AllocRecvBuf(out byte[] buf)
        {
            if (_recvs.TryDequeue(out buf))
                Interlocked.Add(ref _recvCacheSize, -buf.Length);
            else
                buf = new byte[_args.RecvBufSize];
            Interlocked.Add(ref _recvCacheSizeAlloc, buf.Length);
            return eTcpError.Success;
        }

        public eTcpError CollectRecvBuf(byte[] buf)
        {
            if (buf == null)
                return eTcpError.BlockNull;
            if (buf.Length != _args.RecvBufSize)
                return eTcpError.BlockSizeOutOfRange;

            _recvs.Enqueue(buf);
            Interlocked.Add(ref _recvCacheSize, buf.Length);
            Interlocked.Add(ref _recvCacheSizeAlloc, -buf.Length);
            return eTcpError.Success;
        }

        #endregion
    }
}
