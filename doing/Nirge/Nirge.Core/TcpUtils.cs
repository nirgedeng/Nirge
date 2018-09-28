/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Net;
using System;

namespace Nirge.Core
{
    public enum eTcpError
    {
        None,
        Success,
        Unknown,

        SysException,
        SocketError,
        WrongTcpState,

        ArgNull,
        ArgOutOfRange,
        BlockNull,
        BlockSizeIsZero,
        BlockSizeOutOfRange,
        BlockTypeNotSupported,
        CliNull,
        CliOutOfRange,
        SendCacheFull,
        RecvCacheFull,
        SendCacheUsedUp,
        RecvCacheUsedUp,
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
        eTcpError AllocSendBuf(int count, Queue<ArraySegment<byte>> bufs);
        eTcpError CollectSendBuf(byte[] buf);
        eTcpError AllocRecvBuf(out byte[] buf);
        eTcpError CollectRecvBuf(byte[] buf);
    }
}
