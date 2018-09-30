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

        public byte[] AllocSendBuf(int count)
        {
            if (count == 0)
                throw new ArgumentOutOfRangeException("count");

            return new byte[count];
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

        public byte[] AllocRecvBuf(int count)
        {
            if (count == 0)
                throw new ArgumentOutOfRangeException("count");

            return new byte[count];
        }

        public void CollectRecvBuf(byte[] buf)
        {
            if (buf == null)
                throw new ArgumentNullException("buf");
            if (buf.Length == 0)
                throw new ArgumentOutOfRangeException("buf");
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
                return string.Format("STAT CACHE {0} {1} ALLOC {2} {3} " +
                    "S{4} {5} S{6} {7} S{8} {9} S{10} {11} S{12} {13} S{14} {15} S{16} {17} S{16} {18} S{19} {20} S{21} {22} S{23} {24} " +
                    "R{25} {26} R{27} {28} R{29} {30} R{31} {32} R{33} {34} R{35} {36} R{37} {38} R{39} {40} R{41} {42} R{43} {44} R{45} {46}"
                    , _sendCacheSize, _recvCacheSize
                    , _sendCacheSizeAlloc, _recvCacheSizeAlloc
                    , gTcpClientBufSize[0], _sends[0].Count
                    , gTcpClientBufSize[1], _sends[1].Count
                    , gTcpClientBufSize[2], _sends[2].Count
                    , gTcpClientBufSize[3], _sends[3].Count
                    , gTcpClientBufSize[4], _sends[4].Count
                    , gTcpClientBufSize[5], _sends[5].Count
                    , gTcpClientBufSize[6], _sends[6].Count
                    , gTcpClientBufSize[7], _sends[7].Count
                    , gTcpClientBufSize[8], _sends[8].Count
                    , gTcpClientBufSize[9], _sends[9].Count
                    , gTcpClientBufSize[10], _sends[10].Count
                    , gTcpClientBufSize[0], _recvs[0].Count
                    , gTcpClientBufSize[1], _recvs[1].Count
                    , gTcpClientBufSize[2], _recvs[2].Count
                    , gTcpClientBufSize[3], _recvs[3].Count
                    , gTcpClientBufSize[4], _recvs[4].Count
                    , gTcpClientBufSize[5], _recvs[5].Count
                    , gTcpClientBufSize[6], _recvs[6].Count
                    , gTcpClientBufSize[7], _recvs[7].Count
                    , gTcpClientBufSize[8], _recvs[8].Count
                    , gTcpClientBufSize[9], _recvs[9].Count
                    , gTcpClientBufSize[10], _recvs[10].Count);
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
                throw new ArgumentOutOfRangeException("count");

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

            throw new ArgumentOutOfRangeException("count");
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

        public byte[] AllocRecvBuf(int count)
        {
            if (count == 0)
                throw new ArgumentOutOfRangeException("count");

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

            throw new ArgumentOutOfRangeException("count");
        }

        public void CollectRecvBuf(byte[] buf)
        {
            if (buf == null)
                throw new ArgumentNullException("buf");
            if (buf.Length == 0)
                throw new ArgumentOutOfRangeException("buf");

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

            throw new ArgumentOutOfRangeException("buf");
        }

        #endregion
    }
}
