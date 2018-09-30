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
        byte[] AllocSendBuf(int count);
        void CollectSendBuf(byte[] buf);
        byte[] AllocRecvBuf();
        void CollectRecvBuf(byte[] buf);
    }

    public interface ITcpClientPkgHead
    {
        int PkgHeadSize
        {
            get;
        }
        int PkgLen
        {
            get;
            set;
        }
        void Clear();
        void Fill(byte[] buf);
    }

    public interface ITcpClientPkg
    {
        ArraySegment<byte> Fill(int pkgHeadSize, object pkg, ITcpClientCache cache);
    }

    public interface ITcpClientPkgFill
    {
        void Register(Type pkgType, ITcpClientPkg pkg);
        ArraySegment<byte> Fill(int pkgHeadSize, object pkg, ITcpClientCache cache);
    }
}
