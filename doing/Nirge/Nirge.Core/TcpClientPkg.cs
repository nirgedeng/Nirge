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
        const int gPkgHeadSize = 4;

        int _pkgLen;

        public int PkgHeadSize
        {
            get
            {
                return gPkgHeadSize;
            }
        }

        public int PkgLen
        {
            get
            {
                return _pkgLen;
            }
            set
            {
                _pkgLen = value;
            }
        }

        public CTcpClientPkgHead()
        {
            Clear();
        }

        public void Clear()
        {
            _pkgLen = 0;
        }

        public void Fill(byte[] buf)
        {
            if (buf == null)
                throw new ArgumentNullException("buf");
            if (buf.Length < PkgHeadSize)
                throw new ArgumentOutOfRangeException("buf");

            var i = BitConverter.GetBytes(_pkgLen);
            ArrayUtils.Copy(i, 0, buf, 0, PkgHeadSize);
        }
    }

    public class CTcpClientArraySegment : ITcpClientPkg
    {
        public ArraySegment<byte> Fill(int pkgHeadSize, object pkg, ITcpClientCache cache)
        {
            if (pkg == null)
                throw new ArgumentNullException("pkg");
            if (!(pkg is byte[] || pkg is ArraySegment<byte>))
                throw new ArgumentOutOfRangeException("pkg");
            var source = (ArraySegment<byte>)pkg;
            if (source.Count == 0)
                throw new ArgumentOutOfRangeException("pkg");
            if (cache == null)
                throw new ArgumentNullException("cache");

            var pkgSize = pkgHeadSize + source.Count;
            var buf = cache.AllocSendBuf(pkgSize);
            ArrayUtils.Copy(source.Array, source.Offset, buf, pkgHeadSize, source.Count);
            return buf;
        }
    }

    public class CTcpClientPkgFill : ITcpClientPkgFill
    {
        List<Tuple<Type, ITcpClientPkg>> _pkgs;

        public CTcpClientPkgFill()
        {
            _pkgs = new List<Tuple<Type, ITcpClientPkg>>();
        }

        public void Register(Type pkgType, ITcpClientPkg pkg)
        {
            if (pkgType == null)
                throw new ArgumentNullException("pkgType");
            if (pkg == null)
                throw new ArgumentNullException("pkg");

            _pkgs.Add(new Tuple<Type, ITcpClientPkg>(pkgType, pkg));
        }

        public ArraySegment<byte> Fill(int pkgHeadSize, object pkg, ITcpClientCache cache)
        {
            if (pkgHeadSize == 0)
                throw new ArgumentOutOfRangeException("pkgHeadSize");
            if (pkg == null)
                throw new ArgumentNullException("pkg");
            if (cache == null)
                throw new ArgumentNullException("cache");

            foreach (var i in _pkgs)
            {
                if (i.Item1 == pkg.GetType())
                    return i.Item2.Fill(pkgHeadSize, pkg, cache);
            }

            throw new ArgumentOutOfRangeException("pkg");
        }

    }
}
