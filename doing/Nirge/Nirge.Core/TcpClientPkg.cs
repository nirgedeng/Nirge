/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Net;
using System;
using log4net;
using System.Runtime.CompilerServices;

namespace Nirge.Core
{
    public class CTcpClientPkgHead : ITcpClientPkgHead
    {
        const int gPkgHeadSize = 5;

        int _sendPkgType;
        int _sendPkgSize;
        byte[] _recvPkgHeadBuf;
        int _recvPkgType;
        int _recvPkgSize;

        public int PkgHeadSize
        {
            get
            {
                return gPkgHeadSize;
            }
        }

        public int SendPkgType
        {
            get
            {
                return _sendPkgType;
            }
            set
            {
                _sendPkgType = value;
            }
        }

        public int SendPkgSize
        {
            get
            {
                return _sendPkgSize;
            }
            set
            {
                _sendPkgSize = value;
            }
        }

        public byte[] RecvPkgHeadBuf
        {
            get
            {
                return _recvPkgHeadBuf;
            }
        }

        public int RecvPkgType
        {
            get
            {
                return _recvPkgType;
            }
            set
            {
                _recvPkgType = value;
            }
        }

        public int RecvPkgSize
        {
            get
            {
                return _recvPkgSize;
            }
            set
            {
                _recvPkgSize = value;
            }
        }

        public CTcpClientPkgHead()
        {
            _recvPkgHeadBuf = new byte[PkgHeadSize];
            Clear();
        }

        public void Clear()
        {
            _sendPkgType = 0;
            _sendPkgSize = 0;
            _recvPkgType = 0;
            _recvPkgSize = 0;
        }

        public void Fill(byte[] buf)
        {
            if (buf == null)
                throw new ArgumentNullException("buf");
            if (buf.Length < PkgHeadSize)
                throw new ArgumentOutOfRangeException("buf");

            var i = BitConverter.GetBytes(_sendPkgSize);
            buf[0] = i[0];
            buf[1] = i[1];
            buf[2] = i[2];
            buf[3] = i[3];
            buf[4] = (byte)_sendPkgType;
        }

        public void UnFill()
        {
            _recvPkgSize = BitConverter.ToInt32(_recvPkgHeadBuf, 0);
            _recvPkgType = _recvPkgHeadBuf[4];
        }
    }

    public class CTcpClientArraySegment : ITcpClientPkg
    {
        public ArraySegment<byte> Fill(ITcpClientPkgHead pkgHead, int pkgSize, object pkg, ITcpClientCache cache)
        {
            if (pkgHead == null)
                throw new ArgumentNullException("pkgHead");
            if (pkgHead.PkgHeadSize == 0)
                throw new ArgumentOutOfRangeException("pkgHead");
            if (pkgSize == 0)
                throw new ArgumentOutOfRangeException("pkgSize");
            if (pkg == null)
                throw new ArgumentNullException("pkg");
            if (!(pkg is byte[] || pkg is ArraySegment<byte>))
                throw new ArgumentOutOfRangeException("pkg");
            var source = (ArraySegment<byte>)pkg;
            if (source.Count == 0)
                throw new ArgumentOutOfRangeException("pkg");
            if (source.Count > pkgSize)
                throw new ArgumentOutOfRangeException("pkg");
            if (cache == null)
                throw new ArgumentNullException("cache");

            var buf = cache.AllocSendBuf(pkgHead.PkgHeadSize + source.Count);
            ArrayUtils.Copy(source.Array, source.Offset, buf, pkgHead.PkgHeadSize, source.Count);
            pkgHead.SendPkgSize = source.Count;
            pkgHead.SendPkgType = (int)eTcpClientPkgType.ArraySegment;
            return new ArraySegment<byte>(buf, 0, pkgHead.PkgHeadSize + pkgHead.SendPkgSize);
        }

        public object UnFill(ArraySegment<byte> source, ITcpClientCache cache)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (source.Count == 0)
                throw new ArgumentOutOfRangeException("source");
            if (cache == null)
                throw new ArgumentNullException("cache");

            return source;
        }
    }

    public class CTcpClientPkgFill : ITcpClientPkgFill
    {
        List<Tuple<Type, ITcpClientPkg>> _pkgs;
        List<Tuple<int, ITcpClientPkg>> _pkgsV;

        public CTcpClientPkgFill()
        {
            _pkgs = new List<Tuple<Type, ITcpClientPkg>>();
            _pkgsV = new List<Tuple<int, ITcpClientPkg>>();
        }

        public void Register(Type pkgType, int pkgTypeV, ITcpClientPkg pkg)
        {
            if (pkgType == null)
                throw new ArgumentNullException("pkgType");
            if (pkgTypeV == 0)
                throw new ArgumentOutOfRangeException("pkgTypeV");
            if (pkg == null)
                throw new ArgumentNullException("pkg");

            _pkgs.Add(new Tuple<Type, ITcpClientPkg>(pkgType, pkg));
            _pkgsV.Add(new Tuple<int, ITcpClientPkg>(pkgTypeV, pkg));
        }

        public ArraySegment<byte> Fill(ITcpClientPkgHead pkgHead, int pkgSize, object pkg, ITcpClientCache cache)
        {
            if (pkgHead == null)
                throw new ArgumentNullException("pkgHead");
            if (pkgHead.PkgHeadSize == 0)
                throw new ArgumentOutOfRangeException("pkgHead");
            if (pkgSize == 0)
                throw new ArgumentOutOfRangeException("pkgSize");
            if (pkg == null)
                throw new ArgumentNullException("pkg");
            if (cache == null)
                throw new ArgumentNullException("cache");

            foreach (var i in _pkgs)
            {
                if (i.Item1 == pkg.GetType())
                    return i.Item2.Fill(pkgHead, pkgSize, pkg, cache);
            }

            throw new ArgumentOutOfRangeException("pkg");
        }

        public object UnFill(int pkgType, ArraySegment<byte> source, ITcpClientCache cache)
        {
            if (pkgType == 0)
                throw new ArgumentOutOfRangeException("pkgType");
            if (source == null)
                throw new ArgumentNullException("source");
            if (source.Count == 0)
                throw new ArgumentOutOfRangeException("source");
            if (cache == null)
                throw new ArgumentNullException("cache");

            foreach (var i in _pkgsV)
            {
                if (i.Item1 == pkgType)
                    return i.Item2.UnFill(source, cache);
            }

            throw new ArgumentOutOfRangeException("pkg");
        }
    }
}
