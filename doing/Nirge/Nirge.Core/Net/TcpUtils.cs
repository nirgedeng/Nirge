/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

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
        byte[] AllocRecvBuf(int count);
        void CollectRecvBuf(byte[] buf);
    }

    public interface ITcpClientPkgHead
    {
        int PkgHeadSize
        {
            get;
        }
        
        int SendPkgType
        {
            get;
            set;
        }
        
        int SendPkgSize
        {
            get;
            set;
        }
        
        byte[] RecvPkgHeadBuf
        {
            get;
        }
        
        int RecvPkgType
        {
            get;
            set;
        }
        
        int RecvPkgSize
        {
            get;
            set;
        }
        
        void Clear();
        void Fill(byte[] buf);
        void UnFill();
    }

    public enum eTcpClientPkgType
    {
        None,
        ArraySegment,
        Protobuf,
    }

    public interface ITcpClientPkg
    {
        ArraySegment<byte> Fill(ITcpClientPkgHead pkgHead, int gPkgSize, object pkg, ITcpClientCache cache);
        object UnFill(ArraySegment<byte> pkgSeg, ITcpClientCache cache);
    }

    public interface ITcpClientPkgFill
    {
        void Register(Type pkgType, int ePkgType, ITcpClientPkg pkg);
        ArraySegment<byte> Fill(ITcpClientPkgHead pkgHead, int gPkgSize, object pkg, ITcpClientCache cache);
        object UnFill(int pkgType, ArraySegment<byte> pkgSeg, ITcpClientCache cache);
    }
}
