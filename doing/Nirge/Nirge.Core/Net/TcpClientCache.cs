/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Concurrent;
using System.Collections.Generic;
using System;
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
            if (_sendCacheSize > 104857600)
                _sendCacheSize = 104857600;
            if (_recvCacheSize < 10485760)
                _recvCacheSize = 10485760;
            if (_recvCacheSize > 104857600)
                _recvCacheSize = 104857600;
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
                return $"STAT CACHE {_sendCacheSize} {_recvCacheSize} ALLOC {_sendCacheSizeAlloc} {_recvCacheSizeAlloc} " +
                    "S{gTcpClientBufSize[0]} {_sends[0].Count} " +
                    "S{gTcpClientBufSize[1]} {_sends[1].Count} " +
                    "S{gTcpClientBufSize[2]} {_sends[2].Count} " +
                    "S{gTcpClientBufSize[3]} {_sends[3].Count} " +
                    "S{gTcpClientBufSize[4]} {_sends[4].Count} " +
                    "S{gTcpClientBufSize[5]} {_sends[5].Count} " +
                    "S{gTcpClientBufSize[6]} {_sends[6].Count} " +
                    "S{gTcpClientBufSize[7]} {_sends[7].Count} " +
                    "S{gTcpClientBufSize[8]} {_sends[8].Count} " +
                    "S{gTcpClientBufSize[9]} {_sends[9].Count} " +
                    "S{gTcpClientBufSize[10]} {_sends[10].Count} " +
                    "R{gTcpClientBufSize[0]} {_recvs[0].Count} " +
                    "R{gTcpClientBufSize[1]} {_recvs[1].Count} " +
                    "R{gTcpClientBufSize[2]} {_recvs[2].Count} " +
                    "R{gTcpClientBufSize[3]} {_recvs[3].Count} " +
                    "R{gTcpClientBufSize[4]} {_recvs[4].Count} " +
                    "R{gTcpClientBufSize[5]} {_recvs[5].Count} " +
                    "R{gTcpClientBufSize[6]} {_recvs[6].Count} " +
                    "R{gTcpClientBufSize[7]} {_recvs[7].Count} " +
                    "R{gTcpClientBufSize[8]} {_recvs[8].Count} " +
                    "R{gTcpClientBufSize[9]} {_recvs[9].Count} " +
                    "R{gTcpClientBufSize[10]} {_recvs[10].Count} ";
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
