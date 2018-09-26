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

        SysException,
        SocketError,
        WrongTcpState,

        ArgNull,
        ArgOutOfRange,
        BlockNull,
        BlockSizeIsZero,
        BlockSizeOutOfRange,
        CliNull,
        CliOutOfRange,
        SendCacheFull,
        RecvCacheFull,
    }

    public interface ITcpClientCache
    {
        void Clear();
        eTcpError AllocSendBuf(int count, out byte[] buf);
        eTcpError CollectSendBuf(byte[] buf);
        eTcpError AllocRecvBuf(out byte[] buf);
        eTcpError CollectRecvBuf(byte[] buf);
    }
}
