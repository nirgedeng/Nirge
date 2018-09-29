/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Net;
using System;

namespace Nirge.Core
{
    public class CNetException : Exception
    {
        public CNetException()
            :
            base()
        {
        }

        public CNetException(string message)
            :
            base(message)
        {
        }
        public CNetException(string message, Exception innerException)
            :
            base(message, innerException)
        {
        }
    }

    public interface ITcpClientCache
    {
        int SendCacheSize
        {
            get;
        }
        int SendCacheSizeAlloc
        {
            get;
        }
        int RecvBufSize
        {
            get;
        }
        int RecvCacheSize
        {
            get;
        }
        int RecvCacheSizeAlloc
        {
            get;
        }

        bool CanAllocSendBuf
        {
            get;
        }

        bool CanAllocRecvBuf
        {
            get;
        }

        string Stat
        {
            get;
        }

        void Clear();
        void AllocSendBuf(int count, IList<byte[]> bufs);
        void CollectSendBuf(byte[] buf);
        void AllocRecvBuf(out byte[] buf);
        void CollectRecvBuf(byte[] buf);
    }
}
