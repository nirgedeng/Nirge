/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Concurrent;
using System.Collections.Generic;
using System;
using System.Text;
using System.Threading;
using log4net;

namespace Nirge.Core
{
    #region 

    public class CTcpClientCacheArgs
    {
        int _sendCacheSize;
        int _recvCacheSize;

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

        public CTcpClientCacheArgs(int sendCacheSize = 0, int recvCacheSize = 0)
        {
            _sendCacheSize = sendCacheSize;
            _recvCacheSize = recvCacheSize;

            if (_sendCacheSize < 10485760)
                _sendCacheSize = 10485760;
            if (_sendCacheSize > 1073741824)
                _sendCacheSize = 1073741824;
            if (_recvCacheSize < 10485760)
                _recvCacheSize = 10485760;
            if (_recvCacheSize > 1073741824)
                _recvCacheSize = 1073741824;
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
                throw new ArgumentNullException(nameof(args));
            if (log == null)
                throw new ArgumentNullException(nameof(log));

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

        public byte[] AllocSendBuf(int count)
        {
            if (count == 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return new byte[count];
        }

        public void CollectSendBuf(byte[] buf)
        {
            if (buf == null)
                throw new ArgumentNullException(nameof(buf));
            if (buf.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(buf));
        }

        #endregion

        #region 

        public byte[] AllocRecvBuf(int count)
        {
            if (count == 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return new byte[count];
        }

        public void CollectRecvBuf(byte[] buf)
        {
            if (buf == null)
                throw new ArgumentNullException(nameof(buf));
            if (buf.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(buf));
        }

        #endregion
    }

    public class CTcpClientCache : ITcpClientCache
    {
        static readonly int[] gTcpClientBufSize = { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 1048576 };

        CTcpClientCacheArgs _args;
        ILog _log;

        int _sendCacheSize;
        int _sendCacheSizeAlloc;
        ConcurrentQueue<byte[]>[] _sends;
        int _recvCacheSize;
        int _recvCacheSizeAlloc;
        ConcurrentQueue<byte[]>[] _recvs;

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
                var sb = new StringBuilder();
                sb.Append($"STAT CACHE {_sendCacheSize} {_recvCacheSize} ALLOC {_sendCacheSizeAlloc} {_recvCacheSizeAlloc}");
                for (var i = 0; i < gTcpClientBufSize.Length; ++i)
                {
                    if (_sends[i].Count > 0)
                        sb.Append($" S{gTcpClientBufSize[i]} {_sends[i].Count}");
                }
                for (var i = 0; i < gTcpClientBufSize.Length; ++i)
                {
                    if (_recvs[i].Count > 0)
                        sb.Append($" R{gTcpClientBufSize[i]} {_recvs[i].Count}");
                }
                return sb.ToString();
            }
        }

        public CTcpClientCache(CTcpClientCacheArgs args, ILog log)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            _args = args;
            _log = log;

            _sends = new ConcurrentQueue<byte[]>[gTcpClientBufSize.Length];
            for (var i = 0; i < _sends.Length; ++i)
                _sends[i] = new ConcurrentQueue<byte[]>();
            _recvs = new ConcurrentQueue<byte[]>[gTcpClientBufSize.Length];
            for (var i = 0; i < _recvs.Length; ++i)
                _recvs[i] = new ConcurrentQueue<byte[]>();

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
            foreach (var i in _recvs)
            {
                while (i.Count > 0)
                    i.TryDequeue(out buf);
            }
        }

        #region 

        public byte[] AllocSendBuf(int count)
        {
            if (count == 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            byte[] buf;
            for (var i = 0; i < gTcpClientBufSize.Length; ++i)
            {
                if (count > gTcpClientBufSize[i])
                    continue;

                if (_sends[i].TryDequeue(out buf))
                    Interlocked.Add(ref _sendCacheSize, -buf.Length);
                else
                    buf = new byte[gTcpClientBufSize[i]];
                Interlocked.Add(ref _sendCacheSizeAlloc, buf.Length);
                return buf;
            }

            throw new ArgumentOutOfRangeException(nameof(count));
        }

        public void CollectSendBuf(byte[] buf)
        {
            if (buf == null)
                throw new ArgumentNullException(nameof(buf));
            if (buf.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(buf));

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

            throw new ArgumentOutOfRangeException(nameof(buf));
        }

        #endregion

        #region 

        public byte[] AllocRecvBuf(int count)
        {
            if (count == 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            byte[] buf;
            for (var i = 0; i < gTcpClientBufSize.Length; ++i)
            {
                if (count > gTcpClientBufSize[i])
                    continue;

                if (_recvs[i].TryDequeue(out buf))
                    Interlocked.Add(ref _recvCacheSize, -buf.Length);
                else
                    buf = new byte[gTcpClientBufSize[i]];
                Interlocked.Add(ref _recvCacheSizeAlloc, buf.Length);
                return buf;
            }

            throw new ArgumentOutOfRangeException(nameof(count));
        }

        public void CollectRecvBuf(byte[] buf)
        {
            if (buf == null)
                throw new ArgumentNullException(nameof(buf));
            if (buf.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(buf));

            for (var i = 0; i < gTcpClientBufSize.Length; ++i)
            {
                if (buf.Length == gTcpClientBufSize[i])
                {
                    _recvs[i].Enqueue(buf);
                    Interlocked.Add(ref _recvCacheSize, buf.Length);
                    Interlocked.Add(ref _recvCacheSizeAlloc, -buf.Length);
                    return;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(buf));
        }

        #endregion
    }
}
