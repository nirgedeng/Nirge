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
        int _2kSendCapacity;
        int _4kSendCapacity;
        int _8kSendCapacity;
        int _2kRecvCapacity;
        int _4kRecvCapacity;
        int _8kRecvCapacity;

        public int K2SendCapacity
        {
            get
            {
                return _2kSendCapacity;
            }
        }

        public int K4SendCapacity
        {
            get
            {
                return _4kSendCapacity;
            }
        }

        public int K8SendCapacity
        {
            get
            {
                return _8kSendCapacity;
            }
        }

        public int K2RecvCapacity
        {
            get
            {
                return _2kRecvCapacity;
            }
        }

        public int K4RecvCapacity
        {
            get
            {
                return _4kRecvCapacity;
            }
        }

        public int K8RecvCapacity
        {
            get
            {
                return _8kRecvCapacity;
            }
        }

        public CTcpClientCacheArgs(int k2SendCapacity = 0, int k4SendCapacity = 0, int k8SendCapacity = 0, int k2RecvCapacity = 0, int k4RecvCapacity = 0, int k8RecvCapacity = 0)
        {
            _2kSendCapacity = k2SendCapacity;
            _4kSendCapacity = k4SendCapacity;
            _8kSendCapacity = k8SendCapacity;
            _2kRecvCapacity = k2RecvCapacity;
            _4kRecvCapacity = k4RecvCapacity;
            _8kRecvCapacity = k8RecvCapacity;

            if (_2kSendCapacity == 0)
                _2kSendCapacity = 256;
            else if (_2kSendCapacity < 256)
                _2kSendCapacity = 256;
            else if (_2kSendCapacity > 25600)
                _2kSendCapacity = 25600;
            if (_4kSendCapacity == 0)
                _4kSendCapacity = 128;
            else if (_4kSendCapacity < 128)
                _4kSendCapacity = 128;
            else if (_4kSendCapacity > 12800)
                _4kSendCapacity = 12800;
            if (_8kSendCapacity == 0)
                _8kSendCapacity = 64;
            else if (_8kSendCapacity < 64)
                _8kSendCapacity = 64;
            else if (_8kSendCapacity > 6400)
                _8kSendCapacity = 6400;

            if (_2kRecvCapacity == 0)
                _2kRecvCapacity = 256;
            else if (_2kRecvCapacity < 256)
                _2kRecvCapacity = 256;
            else if (_2kRecvCapacity > 25600)
                _2kRecvCapacity = 25600;
            if (_4kRecvCapacity == 0)
                _4kRecvCapacity = 128;
            else if (_4kRecvCapacity < 128)
                _4kRecvCapacity = 128;
            else if (_4kRecvCapacity > 12800)
                _4kRecvCapacity = 12800;
            if (_8kRecvCapacity == 0)
                _8kRecvCapacity = 64;
            else if (_8kRecvCapacity < 64)
                _8kRecvCapacity = 64;
            else if (_8kRecvCapacity > 6400)
                _8kRecvCapacity = 6400;
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

        public byte[] AllocSendBuf(int count)
        {
            return new byte[count];
        }

        public void CollectSendBuf(byte[] buf)
        {
        }

        #endregion

        #region 

        public byte[] AllocRecvBuf(int count)
        {
            return new byte[count];
        }

        public void CollectRecvBuf(byte[] buf)
        {
        }

        #endregion
    }

    public class CTcpClientCache : ITcpClientCache
    {
        const int g2k = 2048;
        const int g4k = 4096;
        const int g8k = 8192;

        CTcpClientCacheArgs _args;

        ConcurrentQueue<byte[]> _2kSends;
        ConcurrentQueue<byte[]> _4kSends;
        ConcurrentQueue<byte[]> _8kSends;

        ConcurrentQueue<byte[]> _2kRecvs;
        ConcurrentQueue<byte[]> _4kRecvs;
        ConcurrentQueue<byte[]> _8kRecvs;

        public CTcpClientCache(CTcpClientCacheArgs args)
        {
            _args = args;

            _2kSends = new ConcurrentQueue<byte[]>();
            _4kSends = new ConcurrentQueue<byte[]>();
            _8kSends = new ConcurrentQueue<byte[]>();

            _2kRecvs = new ConcurrentQueue<byte[]>();
            _4kRecvs = new ConcurrentQueue<byte[]>();
            _8kRecvs = new ConcurrentQueue<byte[]>();
        }

        public void Clear()
        {
            byte[] buf;

            while (_2kSends.Count > 0)
                _2kSends.TryDequeue(out buf);
            while (_4kSends.Count > 0)
                _4kSends.TryDequeue(out buf);
            while (_8kSends.Count > 0)
                _8kSends.TryDequeue(out buf);

            while (_2kRecvs.Count > 0)
                _2kRecvs.TryDequeue(out buf);
            while (_4kRecvs.Count > 0)
                _4kRecvs.TryDequeue(out buf);
            while (_8kRecvs.Count > 0)
                _8kRecvs.TryDequeue(out buf);
        }

        #region 

        public byte[] AllocSendBuf(int count)
        {
            if (count > g8k)
                return new byte[count];
            else if (count > g4k)
            {
                byte[] buf;
                if (_8kSends.TryDequeue(out buf))
                    return buf;
                else
                    return new byte[g8k];
            }
            else if (count > g2k)
            {
                byte[] buf;
                if (_4kSends.TryDequeue(out buf))
                    return buf;
                else
                    return new byte[g4k];
            }
            else
            {
                byte[] buf;
                if (_2kSends.TryDequeue(out buf))
                    return buf;
                else
                    return new byte[g2k];
            }
        }

        public void CollectSendBuf(byte[] buf)
        {
            switch (buf.Length)
            {
            case g2k:
                if (_2kSends.Count < _args.K2SendCapacity)
                    _2kSends.Enqueue(buf);
                break;
            case g4k:
                if (_4kSends.Count < _args.K4SendCapacity)
                    _4kSends.Enqueue(buf);
                break;
            case g8k:
                if (_8kSends.Count < _args.K8SendCapacity)
                    _8kSends.Enqueue(buf);
                break;
            }
        }

        #endregion

        #region 

        public byte[] AllocRecvBuf(int count)
        {
            if (count > g8k)
                return new byte[count];
            else if (count > g4k)
            {
                byte[] buf;
                if (_8kRecvs.TryDequeue(out buf))
                    return buf;
                else
                    return new byte[g8k];
            }
            else if (count > g2k)
            {
                byte[] buf;
                if (_4kRecvs.TryDequeue(out buf))
                    return buf;
                else
                    return new byte[g4k];
            }
            else
            {
                byte[] buf;
                if (_2kRecvs.TryDequeue(out buf))
                    return buf;
                else
                    return new byte[g2k];
            }
        }

        public void CollectRecvBuf(byte[] buf)
        {
            switch (buf.Length)
            {
            case g2k:
                if (_2kRecvs.Count < _args.K2RecvCapacity)
                    _2kRecvs.Enqueue(buf);
                break;
            case g4k:
                if (_4kRecvs.Count < _args.K4RecvCapacity)
                    _4kRecvs.Enqueue(buf);
                break;
            case g8k:
                if (_8kRecvs.Count < _args.K8RecvCapacity)
                    _8kRecvs.Enqueue(buf);
                break;
            }
        }

        #endregion
    }
}
