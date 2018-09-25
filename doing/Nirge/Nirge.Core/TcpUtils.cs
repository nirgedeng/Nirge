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

        Exception,
        SocketError,

        WrongState,
        ArgumentNullRange,
        ArgumentOutOfRange,
        PkgSizeOutOfRange,
        SendQueueFull,
        RecvQueueFull,
        CliOutOfRange,
    }

    public interface ITcpClientCache
    {
        void Clear();
        byte[] AllocSendBuf(int count);
        void CollectSendBuf(byte[] buf);
        byte[] AllocRecvBuf(int count);
        void CollectRecvBuf(byte[] buf);
    }
}
