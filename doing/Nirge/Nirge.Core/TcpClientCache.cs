/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System;
using System.Threading;
using log4net;

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
                _sendCacheSize = 10485760;
            if (_recvCacheSize == 0)
                _recvCacheSize = 10485760;
        }
    }

    #endregion

    public class CTcpClientCacheEmpty : ITcpClientCache
    {
        CTcpClientCacheArgs _args;
        ILog _log;

        int _sendCacheSize;
        int _sendCacheSizeAlloc;
        int _recvCacheSize;
        int _recvCacheSizeAlloc;

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

        public string Stat
        {
            get
            {
                return "";
            }
        }

        public CTcpClientCacheEmpty(CTcpClientCacheArgs args, ILog log)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (log == null)
                throw new ArgumentNullException("log");

            _args = args;
            _log = log;

            Clear();
        }

        public void Clear()
        {
            _sendCacheSize = 0;
            _sendCacheSizeAlloc = 0;

            _recvCacheSize = 0;
            _recvCacheSizeAlloc = 0;
        }

        #region 

        public void AllocSendBuf(int count, IList<byte[]> bufs)
        {
            if (count == 0)
                throw new ArgumentOutOfRangeException("count");
            if (bufs == null)
                throw new ArgumentNullException("bufs");
            if (bufs.Count > 0)
                throw new ArgumentOutOfRangeException("bufs");

            var buf = new byte[count];
            bufs.Add(buf);
        }

        public void CollectSendBuf(byte[] buf)
        {
            if (buf == null)
                throw new ArgumentNullException("buf");
            if (buf.Length == 0)
                throw new ArgumentOutOfRangeException("buf");
        }

        #endregion

        #region 

        public void AllocRecvBuf(out byte[] buf)
        {
            buf = new byte[_args.RecvBufSize];
        }

        public void CollectRecvBuf(byte[] buf)
        {
            if (buf == null)
                throw new ArgumentNullException("buf");
            if (buf.Length != _args.RecvBufSize)
                throw new ArgumentOutOfRangeException("buf");
        }

        #endregion
    }

    public class CTcpClientCache : ITcpClientCache
    {
        static readonly int[] gTcpClientBufSize = { 64, 128, 256, 512, 1024, 2048 };

        CTcpClientCacheArgs _args;
        ILog _log;

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

        public string Stat
        {
            get
            {
                return string.Format("STAT CACHE {0} {1} ALLOC {2} {3} S{4} {5} S{6} {7} S{8} {9} S{10} {11} S{12} {13} S{14} {15} R{16} {17}"
                    , _sendCacheSize, _recvCacheSize
                    , _sendCacheSizeAlloc, _recvCacheSizeAlloc
                    , gTcpClientBufSize[0], _sends[0].Count
                    , gTcpClientBufSize[1], _sends[1].Count
                    , gTcpClientBufSize[2], _sends[2].Count
                    , gTcpClientBufSize[3], _sends[3].Count
                    , gTcpClientBufSize[4], _sends[4].Count
                    , gTcpClientBufSize[5], _sends[5].Count
                    , _args.RecvBufSize, _recvs.Count);
            }
        }

        public CTcpClientCache(CTcpClientCacheArgs args, ILog log)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (log == null)
                throw new ArgumentNullException("log");

            _args = args;
            _log = log;

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

        public void AllocSendBuf(int count, IList<byte[]> bufs)
        {
            if (count == 0)
                throw new ArgumentOutOfRangeException("count");
            if (bufs == null)
                throw new ArgumentNullException("bufs");
            if (bufs.Count > 0)
                throw new ArgumentOutOfRangeException("bufs");

            byte[] buf;

            if (_sendCacheSize > count)
            {
                for (var i = 0; i < gTcpClientBufSize.Length; ++i)
                {
                    if (count > gTcpClientBufSize[i])
                        continue;

                    if (_sends[i].TryDequeue(out buf))
                    {
                        bufs.Add(buf);
                        Interlocked.Add(ref _sendCacheSize, -buf.Length);
                        Interlocked.Add(ref _sendCacheSizeAlloc, buf.Length);
                        return;
                    }
                }
            }

            for (; _sendCacheSize > 0;)
                for (var i = 0; i < _sends.Length; ++i)
                {
                    if (_sends[i].Count > 0
                        && _sends[i].TryDequeue(out buf))
                    {
                        bufs.Add(buf);
                        Interlocked.Add(ref _sendCacheSize, -buf.Length);
                        Interlocked.Add(ref _sendCacheSizeAlloc, buf.Length);
                        if ((count -= buf.Length) <= 0)
                            return;
                    }
                }
            for (; count > 0;)
            {
                for (var i = 0; i < gTcpClientBufSize.Length; ++i)
                {
                    buf = new byte[gTcpClientBufSize[i]];
                    bufs.Add(buf);
                    Interlocked.Add(ref _sendCacheSizeAlloc, buf.Length);
                    if ((count -= buf.Length) <= 0)
                        return;
                }
            }
        }

        public void CollectSendBuf(byte[] buf)
        {
            if (buf == null)
                throw new ArgumentNullException("buf");
            if (buf.Length == 0)
                throw new ArgumentOutOfRangeException("buf");

            for (var i = 0; i < gTcpClientBufSize.Length; ++i)
            {
                if (buf.Length == gTcpClientBufSize[i])
                {
                    _sends[i].Enqueue(buf);
                    Interlocked.Add(ref _sendCacheSize, buf.Length);
                    Interlocked.Add(ref _sendCacheSizeAlloc, -buf.Length);
                    return;
                }
            }

            throw new ArgumentOutOfRangeException("buf");
        }

        #endregion

        #region 

        public void AllocRecvBuf(out byte[] buf)
        {
            if (_recvs.TryDequeue(out buf))
                Interlocked.Add(ref _recvCacheSize, -buf.Length);
            else
                buf = new byte[_args.RecvBufSize];
            Interlocked.Add(ref _recvCacheSizeAlloc, buf.Length);
        }

        public void CollectRecvBuf(byte[] buf)
        {
            if (buf == null)
                throw new ArgumentNullException("buf");
            if (buf.Length != _args.RecvBufSize)
                throw new ArgumentOutOfRangeException("buf");

            _recvs.Enqueue(buf);
            Interlocked.Add(ref _recvCacheSize, buf.Length);
            Interlocked.Add(ref _recvCacheSizeAlloc, -buf.Length);
        }

        #endregion
    }
}
